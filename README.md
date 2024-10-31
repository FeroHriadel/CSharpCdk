# CDK WITH C# STARTER



### SET UP
- download install dotnet
- download and install aws cli (v2)
- create an admin IAM user in AWS Console
- `aws configure` => enter the IAM user's credentials, output: JSON
- npm i -g aws-cdk (v2)
- clone this repo
- `cd CsLambdaHandlers` & `dotnet build --configuration Release`
- `cd ../CsCdk` & `dotnet build --configuration Release`
- `cdk deploy --profile yourprofile`



### PROJECT STRUCTURE (lambda handlers are a separate project)
There seem to have to be 2 projects:
- CsCdk - deploys aws assets. Go to `/CsCdk` and run `cdk deploy --profile csharpadmin`
- CsLambdaHandlers - has handler code for lambdas deployed in CsCdk. You need to `cd CsLambdaHandlers` and `dotnet build --configuration Release` before you deploy.
- reference handler from CsLambdaHandlers in CsCdk like this:

```
CsLambdaHandlers
  /Handlers
    /GetItems.cs
      namespace GetItems {
        public class LambdaFunction {
          public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        }
      }

is referenced in CsCdk:
  var getItemsLambda = new Function(this, "GetItemsLambda", new FunctionProps
  {
      Runtime = Runtime.DOTNET_6,
      Code = Code.FromAsset("../CsLambdaHandlers/bin/Release/net6.0"), //release build will dump lambda handler code here
      Handler = "CsLambdaHandlers::GetItems.LambdaFunction::FunctionHandler", //Project::namespace.class::FunctionHandler
  });
```

