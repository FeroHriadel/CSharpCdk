using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace CsCdkStack
{
    public class ApiGatewayStack : Stack
    {
        public ApiGatewayStack(Constructs.Construct scope, string id, Function getItemsLambda, Function createItemLambda, IStackProps props = null) 
            : base(scope, id, props)
        {
            // Create API Gateway
            var api = new RestApi(this, "MyApi", new RestApiProps
            {
                RestApiName = "CsApi",
                Description = "Testing C# API Gateway"
            });

            // Add items resource and methods
            var itemsResource = api.Root.AddResource("items");
            itemsResource.AddMethod("GET", new LambdaIntegration(getItemsLambda));
            itemsResource.AddMethod("POST", new LambdaIntegration(createItemLambda));
        }
    }
}
