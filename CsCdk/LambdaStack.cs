using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;
using System.Collections.Generic;

namespace CsCdkStack
{
    public class LambdaStack : Stack
    {
        public Function GetItemsLambda { get; private set; }
        public Function CreateItemLambda { get; private set; }

        public LambdaStack(Constructs.Construct scope, string id, Table itemsTable, IStackProps props = null) : base(scope, id, props)
        {
            // Define the GetItemsLambda
            GetItemsLambda = new Function(this, "GetItemsLambda", new FunctionProps
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

            // Define the CreateItemLambda
            CreateItemLambda = new Function(this, "CreateItemLambda", new FunctionProps
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

            // Grant DynamoDB table permissions
            itemsTable.GrantReadData(GetItemsLambda);
            itemsTable.GrantWriteData(CreateItemLambda);
        }
    }
}
