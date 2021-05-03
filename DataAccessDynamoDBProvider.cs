using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DynamoAPI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoAPI
{
    public class DataAccessDynamoDBProvider : IDataAccessProvider
    {
        private readonly ILogger _logger;
        public DataAccessDynamoDBProvider(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("DataAccessDynamoDBProvider");
        }
        public async Task AddUserRecord(User user)
        {
            //Set a local DB context  
            var context = new DynamoDBContext(DynamoDBInitializer.client);
            user.userid = Guid.NewGuid().ToString();
            //Save an User object  
            await context.SaveAsync<User>(user);
        }
        public async Task UpdateUserRecord(User user)
        {
            //Set a local DB context  
            var context = new DynamoDBContext(DynamoDBInitializer.client);
            //Getting an User object  
            List<ScanCondition> conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("userid", ScanOperator.Equal, user.userid));
            var allDocs = await context.ScanAsync<User>(conditions).GetRemainingAsync();
            var editedState = allDocs.FirstOrDefault();
            if (editedState != null)
            {
                editedState = user;
                //Save an User object  
                await context.SaveAsync<User>(editedState);
            }
        }
        public async Task DeleteUserRecord(string userid)
        {
            const string tableName = "User";
            var request = new DeleteItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>() {
                        {
                            "userid",
                            new AttributeValue {
                                S = userid
                            }
                        }
                    }
            };
            var response = await DynamoDBInitializer.client.DeleteItemAsync(request);
        }
        public async Task<User> GetUserSingleRecord(string id)
        {
            var context = new DynamoDBContext(DynamoDBInitializer.client);
            //Getting an User object  
            List<ScanCondition> conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("userid", ScanOperator.Equal, id));
            var allDocs = await context.ScanAsync<User>(conditions).GetRemainingAsync();
            var User = allDocs.FirstOrDefault();
            return User;
        }
        public async Task<IEnumerable<User>> GetUserRecords()
        {
            var context = new DynamoDBContext(DynamoDBInitializer.client);
            //Getting an User object  
            List<ScanCondition> conditions = new List<ScanCondition>();
            var allDocs = await context.ScanAsync<User>(conditions).GetRemainingAsync();
            return allDocs;
        }
    }
}