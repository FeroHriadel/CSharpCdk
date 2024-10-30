using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace CsCdkStack
{
    public class CsStack : Stack
    {
        public CsStack(Constructs.Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var getItemsLambda = new Function(this, "GetItemsLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("../CsLambdaHandlers/bin/Release/net6.0"),
                Handler = "CsLambdaHandlers::GetItems.LambdaFunction::FunctionHandler",
            });
        }
    }
}
