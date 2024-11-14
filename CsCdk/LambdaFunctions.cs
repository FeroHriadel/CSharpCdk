using Amazon.CDK.AWS.Lambda;


namespace CsCdkStack
{
    public class LambdaFunctions
    {
        public Function GetItemsLambda { get; init; }
        public Function CreateItemLambda { get; init; }
        public Function GetUploadLinkLambda { get; init; }
    }
}
