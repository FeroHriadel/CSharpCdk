using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Constructs;

namespace CsCdkStack
{
    public class ApiGatewayStack : Stack
    {
        public ApiGatewayStack(Constructs.Construct scope, string id, LambdaFunctions lambdas, IStackProps props = null) 
            : base(scope, id, props)
        {
            // Create API Gateway with CORS configuration
            var api = new RestApi(this, "MyApi", new RestApiProps
            {
                RestApiName = "CsApi",
                Description = "Testing C# API Gateway",
                DefaultCorsPreflightOptions = new CorsOptions
                {
                    AllowHeaders = new[] { "Content-Type", "X-Amz-Date", "Authorization", "X-Api-Key" },
                    AllowMethods = Cors.ALL_METHODS,   // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
                    AllowCredentials = true,           // Allow credentials (cookies, HTTP authentication, etc.)
                    AllowOrigins = Cors.ALL_ORIGINS    // Allow all origins (can be changed to specific domains)
                }
            });

            // Add items resource and methods
            var itemsResource = api.Root.AddResource("items");
            itemsResource.AddMethod("GET", new LambdaIntegration(lambdas.GetItemsLambda));
            itemsResource.AddMethod("POST", new LambdaIntegration(lambdas.CreateItemLambda));
            
            // Add getuploadlink resource and method
            var getuploadlink = api.Root.AddResource("getuploadlink");
            getuploadlink.AddMethod("POST", new LambdaIntegration(lambdas.GetUploadLinkLambda));
        }
    }
}
