using Amazon.CDK;
using PCDestino.Aws;

var app = new App();
new BackendStack(app, "PCDestino-Backend", new StackProps
{
    Env = new Amazon.CDK.Environment
    {
        Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
        Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
    },
    Description = "PC Destino API, Cognito, ECS Fargate and Aurora PostgreSQL"
});
app.Synth();
