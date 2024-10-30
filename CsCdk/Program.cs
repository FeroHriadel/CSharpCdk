using Amazon.CDK;
using CsCdkStack;

namespace CsCdk
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new CsStack(app, "CsStack");
            app.Synth();
        }
    }
}
