using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.IAM;
using Constructs;
using System.Collections.Generic;

namespace CsCdkStack
{
    public class LambdaStack : Stack
    {
        public LambdaFunctions Lambdas { get; private set; } //class that holds all lambdas (see LambdaFunctions.cs)

        public LambdaStack(Constructs.Construct scope, string id, Table itemsTable, Bucket imagesBucket, IStackProps props = null) 
            : base(scope, id, props)
        {
            var region = this.Region;

            // Get Items Lambda
            var getItemsLambda = new Function(this, "GetItemsLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("../CsLambdaHandlers/bin/Release/net6.0"),
                Handler = "CsLambdaHandlers::GetItems.LambdaFunction::FunctionHandler",
                Timeout = Duration.Seconds(10),
                Environment = new Dictionary<string, string>
                {
                    { "TABLE_NAME", itemsTable.TableName },
                    { "REGION", region }
                }
            });
            itemsTable.GrantReadData(getItemsLambda);

            // Create Item Lambda
            var createItemLambda = new Function(this, "CreateItemLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("../CsLambdaHandlers/bin/Release/net6.0"),
                Handler = "CsLambdaHandlers::CreateItem.LambdaFunction::FunctionHandler",
                Timeout = Duration.Seconds(10),
                Environment = new Dictionary<string, string>
                {
                    { "TABLE_NAME", itemsTable.TableName },
                    { "REGION", region }
                }
            });           
            itemsTable.GrantWriteData(createItemLambda);

            // Get Upload Link Lambda
            var getUploadLinkLambda = new Function(this, "GetUploadLinkLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("../CsLambdaHandlers/bin/Release/net6.0"),
                Handler = "CsLambdaHandlers::GetUploadLink.LambdaFunction::FunctionHandler",
                Timeout = Duration.Seconds(10),
                Environment = new Dictionary<string, string>
                {
                    { "BUCKET_NAME", imagesBucket.BucketName },
                    { "REGION", region }
                }
            });
            var bucketAccessPolicyStatement = new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] { "s3:*" },
                Resources = new[] { imagesBucket.BucketArn }, // or use "arn:aws:s3:::*" if needed
                Effect = Effect.ALLOW
            });
            var bucketAccessPolicy = new Policy(this, "GetUploadLinkLambdaBucketAccess", new PolicyProps
            {
                Statements = new[] { bucketAccessPolicyStatement }
            });
            getUploadLinkLambda.Role?.AttachInlinePolicy(bucketAccessPolicy);
            Amazon.CDK.Tag.Add(getUploadLinkLambda, AppTags.S3AccessTag, AppTags.S3AccessTag);




            // Assign lambdas to the LambdaFunctions class
            Lambdas = new LambdaFunctions
            {
                GetItemsLambda = getItemsLambda,
                CreateItemLambda = createItemLambda,
                GetUploadLinkLambda = getUploadLinkLambda
            };
        }
    }
}
