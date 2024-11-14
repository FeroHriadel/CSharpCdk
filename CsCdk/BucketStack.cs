using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
using Constructs;

namespace CsCdkStack
{
    public class BucketStack : Stack
    {
        public Bucket ImagesBucket { get; private set; }

        public BucketStack(Constructs.Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            string bucketName = (this.StackName + "csharpproject-wvvwvvwvvwvv").ToLower();

            ImagesBucket = new Bucket(this, bucketName, new BucketProps
            {
                BucketName = bucketName,
                ObjectOwnership = ObjectOwnership.BUCKET_OWNER_PREFERRED,
                BlockPublicAccess = new BlockPublicAccess(new BlockPublicAccessOptions
                {
                    BlockPublicAcls = false,
                    IgnorePublicAcls = false,
                    BlockPublicPolicy = false,
                    RestrictPublicBuckets = false
                }),
                Cors = new[]
                {
                    new CorsRule
                    {
                        AllowedMethods = new[]
                        {
                            HttpMethods.HEAD,
                            HttpMethods.GET,
                            HttpMethods.PUT,
                            HttpMethods.POST,
                            HttpMethods.DELETE,
                        },
                        AllowedOrigins = new[] { "*" },
                        AllowedHeaders = new[] { "*" }
                    }
                },
                PublicReadAccess = true,
                RemovalPolicy = RemovalPolicy.DESTROY,
                // AutoDeleteObjects = true //throws an error during deployment
            });

            AddPutObjectStatement();

            AddReadWriteStatement();

            new CfnOutput(this, "ImagesBucketDomainName", new CfnOutputProps
            {
                Value = ImagesBucket.BucketDomainName
            });
        }

        private void AddPutObjectStatement()
        {
            var putObjectStatement = new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new[] { "s3:PutObject" },
                Resources = new[] { $"{ImagesBucket.BucketArn}/*" },
                Principals = new[] { new AnyPrincipal() }
            });

            ImagesBucket.AddToResourcePolicy(putObjectStatement);
        }

        private void AddReadWriteStatement()
        {
            var readWriteStatement = new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new[] { "s3:*" },
                Resources = new[] { $"{ImagesBucket.BucketArn}/*" },
                Principals = new[] { new AnyPrincipal() },
                Conditions = new System.Collections.Generic.Dictionary<string, object>
                {
                    {
                        "StringLike", new System.Collections.Generic.Dictionary<string, object>
                        {
                            { $"aws:PrincipalTag/{AppTags.S3AccessTag}", new[] { AppTags.S3AccessTag } },
                            { "aws:PrincipalArn", new[] { $"arn:aws:iam::{this.Account}:role*" } }
                        }
                    }
                }
            });
            ImagesBucket.AddToResourcePolicy(readWriteStatement);
        }
    }
}
