﻿using Amazon.CDK;

namespace CsCdk
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            // Create DynamoStack and retrieve the table
            var dynamoStack = new CsCdkStack.DynamoStack(app, "DynamoStack");

            // Create LambdaStack and pass the table from DynamoStack
            var lambdaStack = new CsCdkStack.LambdaStack(app, "LambdaStack", dynamoStack.ItemsTable);

            // Create ApiGatewayStack and pass the Lambdas from LambdaFunctions.cs (populated in LambdaStack.cs)
            new CsCdkStack.ApiGatewayStack(app, "ApiGatewayStack", lambdaStack.Lambdas);

            app.Synth();
        }
    }
}

//cdk deploy --all --profile csharpadmin
//cdk deploy DynamoStack LambdaStack --profile myProfile
