using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Text.Json.Serialization;



namespace CreateItem
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
            context.Logger.LogLine("Creating new item in DynamoDB table");

            try
            {
                // Deserialize request body to get 'name'
                var requestBody = JsonSerializer.Deserialize<CreateItemRequest>(request.Body);
                if (string.IsNullOrEmpty(requestBody?.Name))
                {
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 400,
                        Body = "Bad Request: 'name' is required in the body."
                    };
                }

                // Generate unique id
                var id = Guid.NewGuid().ToString();

                // Create item to save
                var newItem = new Dictionary<string, AttributeValue>
                {
                    { "id", new AttributeValue { S = id } },
                    { "name", new AttributeValue { S = requestBody.Name } }
                };

                // Save to DynamoDB
                await _dynamoDbClient.PutItemAsync(_tableName, newItem);

                // Return created item
                return new APIGatewayProxyResponse
                {
                    StatusCode = 201,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" },
                        { "Access-Control-Allow-Origin", "*" },
                        { "Access-Control-Allow-Headers", "Content-Type" }
                    },
                    Body = JsonSerializer.Serialize(new { id, name = requestBody.Name })
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

        public class CreateItemRequest
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }
    }
}
