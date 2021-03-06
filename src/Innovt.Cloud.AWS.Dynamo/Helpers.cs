﻿// INNOVT TECNOLOGIA 2014-2021
// Author: Michel Magalhães
// Project: Innovt.Cloud.AWS.Dynamo
// Solution: Innovt.Platform
// Date: 2021-06-02
// Contact: michel@innovt.com.br or michelmob@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Innovt.Cloud.Table;
using Innovt.Core.Collections;
using Innovt.Core.Utilities;

namespace Innovt.Cloud.AWS.Dynamo
{
    internal static class Helpers
    {
        private const string PaginationTokenSeparator = "|";

        internal static DynamoDBEntry ConvertObjectToDynamoDbEntry(object value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            return DynamoDBEntryConversion.V2.ConvertToEntry(value.ToString());
        }


        public  static string GetTableName<T>()
        {
            if (Attribute.GetCustomAttribute(typeof(T), typeof(DynamoDBTableAttribute)) is not DynamoDBTableAttribute attribute)
                return typeof(T).Name;

            return attribute.TableName;
        }

        private static Dictionary<string, AttributeValue> ConvertToAttributeValues(Dictionary<string, object> items)
        {

            return items?.Select(i =>
                new {
                    i.Key,
                    Value = CreateAttributeValue(i.Value)
                }).ToDictionary(x => x.Key, x => x.Value);

        }

        internal static AttributeValue CreateAttributeValue(object value)
        {
            switch (value)
            {
                case null:
                    return new AttributeValue {NULL = true};
                case MemoryStream stream:
                    return new AttributeValue {B = stream};
                case bool:
                    return new AttributeValue {BOOL = bool.Parse(value.ToString())};
                case List<MemoryStream> streams:
                    return new AttributeValue {BS = streams};
                case List<string> list:
                    return new AttributeValue(list);
                case int or double or float or decimal:
                    return new AttributeValue {N = value.ToString()};
                case DateTime time:
                    return new AttributeValue {S = time.ToString("s")};
                case IList<int> or IList<double> or IList<float> or IList<decimal>:
                {
                    var array = (value as IList).Cast<string>();

                    return new AttributeValue {NS = array.ToList()};
                }
                case IDictionary<string, object> objects:
                {
                    var array = objects.ToDictionary(item => item.Key, item => CreateAttributeValue(item.Value));

                    return new AttributeValue {M = array};
                }
                default:
                    return new AttributeValue(value.ToString());
            }
        }

        private static Dictionary<string, AttributeValue> CreateExpressionAttributeValues(object filter,string attributes)
        {
            if (filter == null)
                return new Dictionary<string, AttributeValue>();

            var attributeValues = new Dictionary<string, AttributeValue>();

            var properties = filter.GetType().GetProperties();

            if (properties.Length <= 0) return attributeValues;

            foreach (var item in properties)
            {
                var key = $":{item.Name}".ToLower(CultureInfo.CurrentCulture);

                if (attributes.Contains(key,StringComparison.InvariantCultureIgnoreCase) && !attributeValues.ContainsKey(key))
                {
                    var value = item.GetValue(filter);

                    attributeValues.Add(key, CreateAttributeValue(value));
                }
            }

            return attributeValues;
        }


        internal static Amazon.DynamoDBv2.Model.QueryRequest CreateQueryRequest<T>(
            Table.QueryRequest request)
        {
            var queryRequest = new Amazon.DynamoDBv2.Model.QueryRequest
            {
                IndexName = request.IndexName,
                TableName = GetTableName<T>(),
                ConsistentRead = request.IndexName == null,
                FilterExpression = request.FilterExpression,
                ScanIndexForward = request.ScanIndexForward,
                KeyConditionExpression = request.KeyConditionExpression,
                ProjectionExpression = request.AttributesToGet,
                ExclusiveStartKey = PaginationTokenToDictionary(request.Page),
                ExpressionAttributeValues = CreateExpressionAttributeValues(request.Filter,
                    string.Join(',', request.KeyConditionExpression, request.FilterExpression))
            };

            if (request.PageSize.HasValue)
                queryRequest.Limit = request.PageSize == 0 ? 1 : request.PageSize.Value;

            return queryRequest;
        }


        internal static Amazon.DynamoDBv2.Model.ScanRequest CreateScanRequest<T>(Table.ScanRequest request)
        {
            var scanRequest = new Amazon.DynamoDBv2.Model.ScanRequest
            {
                IndexName = request.IndexName,
                TableName = GetTableName<T>(),
                ConsistentRead = request.IndexName == null,
                FilterExpression = request.FilterExpression,
                ProjectionExpression = request.AttributesToGet,
                ExclusiveStartKey = PaginationTokenToDictionary(request.Page),
                ExpressionAttributeValues =
                    CreateExpressionAttributeValues(request.Filter, string.Join(',', request.FilterExpression))
            };

            if (request.PageSize.HasValue)
                scanRequest.Limit = request.PageSize == 0 ? 1 : request.PageSize.Value;

            return scanRequest;
        }

        internal static List<T> ConvertAttributesToType<T>(List<Dictionary<string, AttributeValue>> items,
            DynamoDBContext context)
        {
            if (items is null)
                return new List<T>();

            var result = new List<T>();

            items.ForEach(item =>
            {
                var doc = Document.FromAttributeMap(item);
                result.Add(context.FromDocument<T>(doc));
            });

            return result;
        }

        internal static (List<T1> first, List<T2> seccond) ConvertAttributesToType<T1, T2>(
            List<Dictionary<string, AttributeValue>> items, string splitBy, DynamoDBContext context)
        {
            if (items is null)
                return (null, null);

            var result1 = new List<T1>();
            var result2 = new List<T2>();

            foreach (var item in items)
            {
                var doc = Document.FromAttributeMap(item);

                if (item.ContainsKey("EntityType") && item["EntityType"].S == splitBy)
                    result1.Add(context.FromDocument<T1>(doc));
                else
                    result2.Add(context.FromDocument<T2>(doc));
            }

            return (result1, result2);
        }

        internal static (List<T1> first, List<T2> seccond, List<T3> third) ConvertAttributesToType<T1, T2, T3>(
            List<Dictionary<string, AttributeValue>> items, string[] splitBy, DynamoDBContext context)
        {
            if (items is null)
                return (null, null, null);

            var result1 = new List<T1>();
            var result2 = new List<T2>();
            var result3 = new List<T3>();

            foreach (var item in items)
            {
                var doc = Document.FromAttributeMap(item);

                if (!item.ContainsKey("EntityType"))
                    continue;

                if (item["EntityType"].S == splitBy[0])
                {
                    result1.Add(context.FromDocument<T1>(doc));
                }
                else
                {
                    if (item["EntityType"].S == splitBy[1])
                        result2.Add(context.FromDocument<T2>(doc));
                    else
                        result3.Add(context.FromDocument<T3>(doc));
                }
            }

            return (result1, result2, result3);
        }

        internal static string CreatePaginationToken(Dictionary<string, AttributeValue> lastEvaluatedKey)
        {
            if (lastEvaluatedKey.IsNullOrEmpty())
                return null;

            var stringBuilder = new StringBuilder();

            foreach (var (attributeKey, attributeValue) in lastEvaluatedKey)
            {
                var value = attributeValue.S != null ? $"S:{attributeValue.S}" : $"N:{attributeValue.N}";

                stringBuilder.Append($"{attributeKey}{PaginationTokenSeparator}{value}\\r\\n");
            }

            return Convert.ToBase64String(stringBuilder.ToString().Zip()).UrlEncode();
        }

        private static Dictionary<string, AttributeValue> PaginationTokenToDictionary(string paginationToken)
        {
            if (paginationToken is null)
                return null;

            var decryptedToken = Convert.FromBase64String(paginationToken.UrlDecode()).Unzip();

            var result = new Dictionary<string, AttributeValue>();

            var keys = decryptedToken.Split("\\r\\n");

            foreach (var key in keys)
            {
                var attributes = key.Split(PaginationTokenSeparator);

                if (attributes.Length < 2)
                    continue;

                var attributeKey = attributes[0];
                //Just in case that the separator was used
                var attributeValue = string.Join(PaginationTokenSeparator, attributes, 1, attributes.Length - 1);

                if (attributeValue.StartsWith("S:", StringComparison.OrdinalIgnoreCase))
                    result.Add(attributeKey,
                        new AttributeValue(attributeValue[2..]));
                else
                    result.Add(attributeKey, new AttributeValue
                    {
                        N = attributeValue[2..]
                    });
            }

            return result;
        }

        private static Put CreatePutTransactItem(TransactionWriteItem transactionWriteItem) { 

            if(transactionWriteItem.OperationType != TransactionWriteOperationType.Put)
                return null;

            return new Put()
            {
                ConditionExpression = transactionWriteItem.ConditionExpression,
                TableName = transactionWriteItem.TableName,
                ExpressionAttributeValues = ConvertToAttributeValues(transactionWriteItem.ExpressionAttributeValues),
                Item = ConvertToAttributeValues(transactionWriteItem.Keys)                
            };
        }

       

        private static ConditionCheck CreateConditionCheckTransactItem(TransactionWriteItem transactionWriteItem)
        {
            if (transactionWriteItem.OperationType != TransactionWriteOperationType.Check)
                return null;

            return new ConditionCheck()
            {
                ConditionExpression = transactionWriteItem.ConditionExpression,
                TableName = transactionWriteItem.TableName,
                ExpressionAttributeValues = ConvertToAttributeValues(transactionWriteItem.ExpressionAttributeValues),
                Key = ConvertToAttributeValues(transactionWriteItem.Keys),
            };
        }
        
        private static Delete CreateDeleteTransactItem(TransactionWriteItem transactionWriteItem)
        {
            if (transactionWriteItem.OperationType != TransactionWriteOperationType.Delete)
                return null;

            return new Delete()
            {
                ConditionExpression = transactionWriteItem.ConditionExpression,
                TableName = transactionWriteItem.TableName,
                ExpressionAttributeValues = ConvertToAttributeValues(transactionWriteItem.ExpressionAttributeValues),
                Key = ConvertToAttributeValues(transactionWriteItem.Keys)                
            };
        }

        private static Update CreateUpdateTransactItem(TransactionWriteItem transactionWriteItem)
        {
            if (transactionWriteItem.OperationType != TransactionWriteOperationType.Update)
                return null;

            return new Update()
            {
                ConditionExpression = transactionWriteItem.ConditionExpression,
                TableName = transactionWriteItem.TableName,
                UpdateExpression = transactionWriteItem.UpdateExpression,
                ExpressionAttributeValues = ConvertToAttributeValues(transactionWriteItem.ExpressionAttributeValues),
                Key = ConvertToAttributeValues(transactionWriteItem.Keys)
            };
        }

        internal static TransactWriteItem CreateTransactionItem(TransactionWriteItem transactionWriteItem)
        {
            if (transactionWriteItem is null)
            {
                throw new ArgumentNullException(nameof(transactionWriteItem));
            }

            return  new TransactWriteItem
            {
                ConditionCheck = CreateConditionCheckTransactItem(transactionWriteItem),
                Put = CreatePutTransactItem(transactionWriteItem),
                Delete = CreateDeleteTransactItem(transactionWriteItem),
                Update = CreateUpdateTransactItem(transactionWriteItem)
            };
        }
    }
}