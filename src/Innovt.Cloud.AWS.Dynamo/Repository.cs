﻿using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Innovt.Cloud.AWS.Configuration;
using Innovt.Cloud.Table;
using Innovt.Core.CrossCutting.Log;
using Polly.Retry;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Innovt.Core.Collections;

namespace Innovt.Cloud.AWS.Dynamo
{
    public abstract class Repository : AwsBaseService, ITableRepository
    {
        protected Repository(ILogger logger, IAWSConfiguration configuration) : base(logger, configuration)
        {
        }

        protected Repository(ILogger logger, IAWSConfiguration configuration, string region) : base(logger, configuration, region)
        {
        }

        private DynamoDBContext context = null;
        private DynamoDBContext Context => context ??= new DynamoDBContext(DynamoClient);

        private AmazonDynamoDBClient dynamoClient = null;
        private AmazonDynamoDBClient DynamoClient => dynamoClient ??= CreateService<AmazonDynamoDBClient>();

        protected override AsyncRetryPolicy CreateDefaultRetryAsyncPolicy()
        {
            return base.CreateRetryAsyncPolicy<ProvisionedThroughputExceededException,
                                               InternalServerErrorException,LimitExceededException, ResourceInUseException>();
        }

        public async Task<T> GetByIdAsync<T>(object id, string rangeKey = null, CancellationToken cancellationToken = default) where T : ITableMessage
        {
            var config = new DynamoDBOperationConfig()
            {
                ConsistentRead = true
            };

            var policy = this.CreateDefaultRetryAsyncPolicy();

            if (string.IsNullOrEmpty(rangeKey))
                return await policy.ExecuteAsync(async () => await Context.LoadAsync<T>(id, config, cancellationToken)).ConfigureAwait(false);

            return await policy.ExecuteAsync(async () => await Context.LoadAsync<T>(id, rangeKey, config, cancellationToken)).ConfigureAwait(false);
        }

        public async Task DeleteAsync<T>(T value, CancellationToken cancellationToken = default) where T : ITableMessage
        {
            await this.CreateDefaultRetryAsyncPolicy().ExecuteAsync(async () => await Context.DeleteAsync<T>(value, cancellationToken)).ConfigureAwait(false);
        }

        public async Task DeleteAsync<T>(object id, string rangeKey=null, CancellationToken cancellationToken = default) where T : ITableMessage
        {
            var policy = this.CreateDefaultRetryAsyncPolicy();

            if (string.IsNullOrEmpty(rangeKey))
                await policy.ExecuteAsync(async () => await Context.DeleteAsync<T>(id, cancellationToken)).ConfigureAwait(false);
            else
                await policy.ExecuteAsync(async () => await Context.DeleteAsync<T>(id, rangeKey, cancellationToken)).ConfigureAwait(false);
        }

        public async Task AddAsync<T>(T message, CancellationToken cancellationToken = default) where T : ITableMessage
        {
            var config = new DynamoDBOperationConfig()
            {
                IgnoreNullValues = true
            };

            await this.CreateDefaultRetryAsyncPolicy().ExecuteAsync(async () => await Context.SaveAsync(message, config, cancellationToken)).ConfigureAwait(false);
        }

        public async Task AddAsync<T>(IList<T> messages, CancellationToken cancellationToken = default) where T : ITableMessage
        {
            if (messages is null) throw new System.ArgumentNullException(nameof(messages));
           
            var batch = Context.CreateBatchWrite<T>();

            batch.AddPutItems(messages);

            await this.CreateDefaultRetryAsyncPolicy().ExecuteAsync(async () => await batch.ExecuteAsync(cancellationToken)).ConfigureAwait(false);
        }

        protected async Task UpdateAsync(string tableName, Dictionary<string, AttributeValue> key,
            Dictionary<string, AttributeValueUpdate> attributeUpdates,
            CancellationToken cancellationToken = default)
        {
            await CreateDefaultRetryAsyncPolicy().ExecuteAsync(async () => 
            await DynamoClient.UpdateItemAsync(tableName, key, attributeUpdates, cancellationToken)).ConfigureAwait(false);
        }

        public async Task<T> QueryFirstAsync<T>(object id, CancellationToken cancellationToken = default) where T : ITableMessage
        {
            var config = new DynamoDBOperationConfig()
            {
                BackwardQuery = true
            };

            var result = await CreateDefaultRetryAsyncPolicy().ExecuteAsync(async () => 
                         await Context.QueryAsync<T>(id, config).GetNextSetAsync(cancellationToken)).ConfigureAwait(false);

            if (result == null)
                return default;

            return result.FirstOrDefault();
        }

        public async Task<IList<T>> QueryAsync<T>(object id, CancellationToken cancellationToken = default) where T : ITableMessage
        {
            var config = new DynamoDBOperationConfig()
            {
                ConsistentRead = true,
                BackwardQuery = true,
            };

            var result = await CreateDefaultRetryAsyncPolicy().ExecuteAsync(async () =>
                          await Context.QueryAsync<T>(id, config).GetRemainingAsync(cancellationToken)).ConfigureAwait(false);

            return result;
        }
 
        public async Task<PagedCollection<T>> QueryPaginatedByAsync<T>(Innovt.Cloud.Table.QueryRequest request, CancellationToken cancellationToken = default) where T : ITableMessage
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var queryRequest = Helpers.CreateQueryRequest<T>(request);

            var queryResponse = await DynamoClient.QueryAsync(queryRequest).ConfigureAwait(false);

            var a = queryResponse.Count;

            if (queryResponse.Items is null)
                return null;

            return new PagedCollection<T>()
            {
                Items = Helpers.ConvertAttributesToType<T>(queryResponse.Items, Context),
                Page = queryResponse.Items?.Count() > 0 ? Helpers.CreatePaginationToken(queryResponse.LastEvaluatedKey) : null
            };
        }

        public async Task<PagedCollection<T>> ScanPaginatedByAsync<T>(Innovt.Cloud.Table.ScanRequest request, CancellationToken cancellationToken = default) where T : ITableMessage
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var scanRequest  = Helpers.CreateScanRequest<T>(request);

            var scanResponse = await DynamoClient.ScanAsync(scanRequest).ConfigureAwait(false);

            if (scanResponse.Items is null)
                return null;


            var response = new PagedCollection<T>()
            {
                Items = Helpers.ConvertAttributesToType<T>(scanResponse.Items, Context),
                Page = scanResponse.Items?.Count() > 0 ? Helpers.CreatePaginationToken(scanResponse.LastEvaluatedKey) : null
            };

            return response;
        }

        protected async Task<TransactGetItemsResponse> TransactGetItemsAsync<T>(TransactGetItemsRequest request, CancellationToken cancellationToken = default) where T : ITableMessage
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            return await this.CreateDefaultRetryAsyncPolicy().ExecuteAsync(async () => await DynamoClient.TransactGetItemsAsync(request, cancellationToken)).ConfigureAwait(false);
        }

        protected override void DisposeServices()
        {
            context?.Dispose();
            dynamoClient?.Dispose();
        }
    }
}