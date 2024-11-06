using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;

namespace CsCdkStack
{
    public class DynamoStack : Stack
    {
        public Table ItemsTable { get; private set; }

        public DynamoStack(Constructs.Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            ItemsTable = new Table(this, "ItemsTable", new TableProps
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
        }
    }
}
