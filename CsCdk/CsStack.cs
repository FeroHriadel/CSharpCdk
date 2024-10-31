using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;

namespace CsCdkStack
{
    public class CsStack : Stack
    {
        public CsStack(Constructs.Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            //create dynamoDB table:
            var itemsTable = new Table(this, "ItemsTable", new TableProps
            {
                TableName = "Items",
                PartitionKey = new Amazon.CDK.AWS.DynamoDB.Attribute
                {
                    Name = "id",
                    Type = AttributeType.STRING
                },
                BillingMode = BillingMode.PAY_PER_REQUEST,
                RemovalPolicy = RemovalPolicy.DESTROY
            });


            //create lambdas:
            var getItemsLambda = new Function(this, "GetItemsLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("../CsLambdaHandlers/bin/Release/net6.0"),
                Handler = "CsLambdaHandlers::GetItems.LambdaFunction::FunctionHandler",
                Timeout = Duration.Seconds(10),
                Environment = new Dictionary<string, string>
                {
                    { "TABLE_NAME", itemsTable.TableName }
                }
            });

            var createItemLambda = new Function(this, "CreateItemLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("../CsLambdaHandlers/bin/Release/net6.0"),
                Handler = "CsLambdaHandlers::CreateItem.LambdaFunction::FunctionHandler",
                Timeout = Duration.Seconds(10),
                Environment = new Dictionary<string, string>
                {
                    { "TABLE_NAME", itemsTable.TableName }
                }
            });


            //add table rights to lambdas:
            itemsTable.GrantReadData(getItemsLambda);
            itemsTable.GrantWriteData(createItemLambda);


            //create api gateway:
            var api = new RestApi(this, "MyApi", new RestApiProps
            {
                RestApiName = "CsApi",
                Description = "testing Csharp apigw"
            });


            //add endpoints to apigateway:
            var getItemsResource = api.Root.AddResource("items");
            getItemsResource.AddMethod("GET", new LambdaIntegration(getItemsLambda));
            getItemsResource.AddMethod("POST", new LambdaIntegration(createItemLambda));
        }
    }
}
