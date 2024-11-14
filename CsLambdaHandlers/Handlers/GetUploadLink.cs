using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.S3.Transfer;



namespace GetUploadLink
{
    public class LambdaFunction
    {
        private readonly string _bucketName;
        private readonly AmazonS3Client _s3Client;

        public LambdaFunction()
        {
            _bucketName = Environment.GetEnvironmentVariable("BUCKET_NAME");
            var region = Environment.GetEnvironmentVariable("REGION");
            _s3Client = new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(region));
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                var body = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);
                if (!body.TryGetValue("fileName", out var fileName) || string.IsNullOrWhiteSpace(fileName))
                {
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 400,
                        Body = "fileName is required"
                    };
                }

                // Clean up file name and generate unique key
                fileName = fileName.Replace(" ", "");
                var uniqueKey = $"{fileName}";
                var expiresInSeconds = 300;

                // Generate presigned URL
                var url = GeneratePresignedUrl(uniqueKey, expiresInSeconds);
                
                context.Logger.LogLine($"Presigned URL generated: {url}");

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" },
                        { "Access-Control-Allow-Origin", "*" },
                        { "Access-Control-Allow-Headers", "Content-Type" }
                    },
                    Body = JsonSerializer.Serialize(new { url })
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error: {ex.Message}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = "Something went wrong"
                };
            }
        }

        private string GeneratePresignedUrl(string key, int expiresIn)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddSeconds(expiresIn)
            };

            return _s3Client.GetPreSignedURL(request);
        }
    }
}
