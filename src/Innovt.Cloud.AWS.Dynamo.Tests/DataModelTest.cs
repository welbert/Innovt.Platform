// INNOVT TECNOLOGIA 2014-2021
// Author: Michel Magalh�es
// Project: Innovt.Cloud.AWS.Dynamo.Tests
// Solution: Innovt.Platform
// Date: 2021-06-02
// Contact: michel@innovt.com.br or michelmob@gmail.com

using Amazon.DynamoDBv2.DataModel;

namespace Innovt.Cloud.AWS.Dynamo.Tests
{
    [DynamoDBTable("Invoices")]
    public class DataModelTest
    {
        // [DynamoDBHashKey]
        public string BuyerId { get; set; }

        //[DynamoDBRangeKey]
        public string PaymentOrderErpId { get; set; }

        public string PaymentOrderStatus { get; set; }

        public string PaymentOrderStatusId { get; set; }

        public decimal Tax { get; set; }
    }
}