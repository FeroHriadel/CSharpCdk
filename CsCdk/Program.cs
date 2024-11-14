using Amazon.CDK;

namespace CsCdk
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            var dynamoStack = new CsCdkStack.DynamoStack(app, "DynamoStack");

            var bucketStack = new CsCdkStack.BucketStack(app, "BucketStack");

            var lambdaStack = new CsCdkStack.LambdaStack(app, "LambdaStack", dynamoStack.ItemsTable, bucketStack.ImagesBucket);


            new CsCdkStack.ApiGatewayStack(app, "ApiGatewayStack", lambdaStack.Lambdas);

            app.Synth();
        }
    }
}

//cdk deploy --all --profile csharpadmin
//cdk deploy DynamoStack LambdaStack --profile myProfile
