using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DynamoAPI
{
    public static class DynamoDBInitializer
    {
        private const string accessKey = "AKIA4QZD7CUSAWBKOQXV";
        private const string secretKey = "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";
        public static AmazonDynamoDBClient client;
        public static async Task InitializeDynamoDB()
        {
            string tableName = "User";
            string hashKey = "userid";

            //Creating credentials and initializing DynamoDB client  
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);

            //Verify table  
            var tableResponse = await client.ListTablesAsync();
            if (!tableResponse.TableNames.Contains(tableName))
            {
                //Table not found, creating table  
                await client.CreateTableAsync(new CreateTableRequest
                {
                    TableName = tableName,
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 3,
                        WriteCapacityUnits = 1
                    },
                    KeySchema = new List<KeySchemaElement> {
                            new KeySchemaElement {
                                AttributeName = hashKey,
                                KeyType = KeyType.HASH
                            }
                        },
                    AttributeDefinitions = new List<AttributeDefinition> {
                                                        new AttributeDefinition {
                                AttributeName = hashKey, AttributeType = ScalarAttributeType.S
                            }
                        }
                });
                bool isTableAvailable = false;
                while (!isTableAvailable)
                {
                    //"Waiting for table to be active...  
                    Thread.Sleep(5000);
                    var tableStatus = await client.DescribeTableAsync(tableName);
                    isTableAvailable = tableStatus.Table.TableStatus == "ACTIVE";
                }
            }
        }
    }
}