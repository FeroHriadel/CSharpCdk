using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GetItems
{
    public class LambdaFunction
    {
        private readonly string _tableName;
        private readonly AmazonDynamoDBClient _dynamoDbClient;

        public LambdaFunction()
        {
            _dynamoDbClient = new AmazonDynamoDBClient();
            _tableName = Environment.GetEnvironmentVariable("TABLE_NAME");
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine($"Fetching items from DynamoDB table: {_tableName}");
            try
            {
                var items = await GetItemsFromDynamoDB();
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" },
                        { "Access-Control-Allow-Origin", "*" },
                        { "Access-Control-Allow-Headers", "Content-Type" }
                    },
                    Body = System.Text.Json.JsonSerializer.Serialize(new { items })
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error: {ex.Message}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = "Internal Server Error"
                };
            }
        }

        private async Task<List<Dictionary<string, string>>> GetItemsFromDynamoDB()
        {
            var items = new List<Dictionary<string, string>>();

            var request = new ScanRequest
            {
                TableName = _tableName
            };

            var response = await _dynamoDbClient.ScanAsync(request);

            foreach (var item in response.Items)
            {
                var dict = new Dictionary<string, string>();
                foreach (var kvp in item)
                {
                    dict[kvp.Key] = kvp.Value.S; // Adjust according to your attribute types
                }
                items.Add(dict);
            }

            return items;
        }
    }
}
