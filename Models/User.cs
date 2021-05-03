using Amazon.DynamoDBv2.DataModel;

namespace DynamoAPI.Models
{
    [DynamoDBTable("User")]
    public class User
    {
        [DynamoDBHashKey]
        public string userid { get; set; }
        public string username { get; set; }
        public string age { get; set; }
    }
}