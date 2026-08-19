using Amazon.CDK;
using Amazon.CDK.AWS.ApplicationAutoScaling;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.WAFv2;
using Constructs;

namespace PCDestino.Aws;

public sealed class BackendStack : Stack
{
    public BackendStack(Construct scope, string id, IStackProps? props = null) : base(scope, id, props)
    {
        var certificateArn = new CfnParameter(this, "CertificateArn", new CfnParameterProps
        {
            Type = "String",
            Description = "ARN de um certificado ACM válido para o domínio público da API"
        });

        var vpc = new Vpc(this, "Vpc", new VpcProps
        {
            MaxAzs = 2,
            NatGateways = 1,
            SubnetConfiguration =
            [
                new SubnetConfiguration { Name = "public", SubnetType = SubnetType.PUBLIC, CidrMask = 24 },
                new SubnetConfiguration { Name = "application", SubnetType = SubnetType.PRIVATE_WITH_EGRESS, CidrMask = 24 },
                new SubnetConfiguration { Name = "database", SubnetType = SubnetType.PRIVATE_ISOLATED, CidrMask = 24 }
            ]
        });

        var databaseSecurityGroup = new SecurityGroup(this, "DatabaseSecurityGroup", new SecurityGroupProps
        {
            Vpc = vpc,
            AllowAllOutbound = false,
            Description = "Only API tasks can reach PostgreSQL"
        });
        var database = new DatabaseCluster(this, "Database", new DatabaseClusterProps
        {
            Engine = DatabaseClusterEngine.AuroraPostgres(new AuroraPostgresClusterEngineProps
            {
                Version = AuroraPostgresEngineVersion.VER_17_7
            }),
            Writer = ClusterInstance.ServerlessV2("writer"),
            Readers = [ClusterInstance.ServerlessV2("reader", new ServerlessV2ClusterInstanceProps { ScaleWithWriter = true })],
            ServerlessV2MinCapacity = 0.5,
            ServerlessV2MaxCapacity = 8,
            Credentials = Credentials.FromGeneratedSecret("pcdestino_admin"),
            DefaultDatabaseName = "pcdestino",
            StorageEncrypted = true,
            DeletionProtection = true,
            Backup = new BackupProps
            {
                Retention = Duration.Days(14),
                PreferredWindow = "03:00-04:00"
            },
            PreferredMaintenanceWindow = "sun:04:00-sun:05:00",
            Vpc = vpc,
            VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_ISOLATED },
            SecurityGroups = [databaseSecurityGroup],
            CloudwatchLogsExports = ["postgresql"],
            CloudwatchLogsRetention = RetentionDays.ONE_MONTH
        });
        database.ApplyRemovalPolicy(RemovalPolicy.RETAIN);

        var userPool = new UserPool(this, "Users", new UserPoolProps
        {
            UserPoolName = "pcdestino-users",
            SelfSignUpEnabled = true,
            SignInAliases = new SignInAliases { Email = true },
            AutoVerify = new AutoVerifiedAttrs { Email = true },
            AccountRecovery = AccountRecovery.EMAIL_ONLY,
            Mfa = Mfa.OPTIONAL,
            MfaSecondFactor = new MfaSecondFactor { Otp = true, Sms = false },
            PasswordPolicy = new PasswordPolicy
            {
                MinLength = 12,
                RequireDigits = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireSymbols = true,
                TempPasswordValidity = Duration.Days(3)
            },
            RemovalPolicy = RemovalPolicy.RETAIN
        });
        userPool.AddDomain("ManagedLoginDomain", new UserPoolDomainOptions
        {
            CognitoDomain = new CognitoDomainOptions { DomainPrefix = $"pcdestino-{Amazon.CDK.Aws.ACCOUNT_ID}" }
        });
        var userPoolClient = userPool.AddClient("MobileClient", new UserPoolClientOptions
        {
            UserPoolClientName = "pcdestino-mobile",
            GenerateSecret = false,
            PreventUserExistenceErrors = true,
            AuthFlows = new AuthFlow { UserSrp = true },
            OAuth = new OAuthSettings
            {
                Flows = new OAuthFlows { AuthorizationCodeGrant = true },
                Scopes = [OAuthScope.OPENID, OAuthScope.EMAIL, OAuthScope.PROFILE],
                CallbackUrls = ["pcdestino://auth/callback", "http://localhost:8081/auth/callback"],
                LogoutUrls = ["pcdestino://auth/logout", "http://localhost:8081/"]
            },
            SupportedIdentityProviders = [UserPoolClientIdentityProvider.COGNITO],
            AccessTokenValidity = Duration.Hours(1),
            IdTokenValidity = Duration.Hours(1),
            RefreshTokenValidity = Duration.Days(30)
        });
        _ = new CfnUserPoolGroup(this, "ModeratorGroup", new CfnUserPoolGroupProps
        {
            UserPoolId = userPool.UserPoolId,
            GroupName = "Moderator",
            Description = "Pode moderar contribuições e avaliações"
        });
        _ = new CfnUserPoolGroup(this, "AdminGroup", new CfnUserPoolGroupProps
        {
            UserPoolId = userPool.UserPoolId,
            GroupName = "Admin",
            Description = "Administração restrita da plataforma"
        });

        var cluster = new Cluster(this, "Cluster", new ClusterProps
        {
            Vpc = vpc,
            ContainerInsightsV2 = ContainerInsights.ENABLED,
            EnableFargateCapacityProviders = true
        });
        var sourcePath = (string?)Node.TryGetContext("sourcePath") ?? "..";
        var certificate = Certificate.FromCertificateArn(this, "Certificate", certificateArn.ValueAsString);
        var service = new ApplicationLoadBalancedFargateService(this, "Api", new ApplicationLoadBalancedFargateServiceProps
        {
            Cluster = cluster,
            Cpu = 512,
            MemoryLimitMiB = 1024,
            DesiredCount = 2,
            PublicLoadBalancer = true,
            Protocol = ApplicationProtocol.HTTPS,
            Certificate = certificate,
            RedirectHTTP = true,
            TaskSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_WITH_EGRESS },
            AssignPublicIp = false,
            CircuitBreaker = new DeploymentCircuitBreaker { Rollback = true },
            MinHealthyPercent = 100,
            HealthCheckGracePeriod = Duration.Seconds(60),
            TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
            {
                Image = ContainerImage.FromAsset(sourcePath, new AssetImageProps { File = "backend/Dockerfile" }),
                ContainerPort = 8080,
                Environment = new Dictionary<string, string>
                {
                    ["ASPNETCORE_ENVIRONMENT"] = "Production",
                    ["ASPNETCORE_HTTP_PORTS"] = "8080",
                    ["Database__Host"] = database.ClusterEndpoint.Hostname,
                    ["Database__Port"] = database.ClusterEndpoint.Port.ToString(),
                    ["Database__Name"] = "pcdestino",
                    ["Database__RequireSsl"] = "true",
                    ["Database__RunMigrationsOnStartup"] = "false",
                    ["Database__SeedDemoData"] = "false",
                    ["Authentication__Mode"] = "Cognito",
                    ["Authentication__Authority"] = $"https://cognito-idp.{Amazon.CDK.Aws.REGION}.amazonaws.com/{userPool.UserPoolId}",
                    ["Authentication__ClientId"] = userPoolClient.UserPoolClientId,
                    ["OpenApi__Enabled"] = "false"
                },
                Secrets = new Dictionary<string, Amazon.CDK.AWS.ECS.Secret>
                {
                    ["Database__Username"] = Amazon.CDK.AWS.ECS.Secret.FromSecretsManager(database.Secret!, "username"),
                    ["Database__Password"] = Amazon.CDK.AWS.ECS.Secret.FromSecretsManager(database.Secret!, "password")
                },
                LogDriver = LogDrivers.AwsLogs(new AwsLogDriverProps
                {
                    StreamPrefix = "pcdestino-api",
                    LogRetention = RetentionDays.ONE_MONTH
                })
            }
        });
        service.TargetGroup.ConfigureHealthCheck(new Amazon.CDK.AWS.ElasticLoadBalancingV2.HealthCheck
        {
            Path = "/health/live",
            HealthyHttpCodes = "200",
            Interval = Duration.Seconds(30),
            Timeout = Duration.Seconds(5)
        });
        database.Connections.AllowDefaultPortFrom(service.Service, "API to Aurora PostgreSQL");

        var scaling = service.Service.AutoScaleTaskCount(new EnableScalingProps { MinCapacity = 2, MaxCapacity = 10 });
        scaling.ScaleOnCpuUtilization("CpuScaling", new CpuUtilizationScalingProps
        {
            TargetUtilizationPercent = 60,
            ScaleInCooldown = Duration.Seconds(120),
            ScaleOutCooldown = Duration.Seconds(60)
        });
        scaling.ScaleOnMemoryUtilization("MemoryScaling", new MemoryUtilizationScalingProps
        {
            TargetUtilizationPercent = 70,
            ScaleInCooldown = Duration.Seconds(120),
            ScaleOutCooldown = Duration.Seconds(60)
        });

        var webAcl = new CfnWebACL(this, "WebAcl", new CfnWebACLProps
        {
            Scope = "REGIONAL",
            DefaultAction = new CfnWebACL.DefaultActionProperty { Allow = new CfnWebACL.AllowActionProperty() },
            VisibilityConfig = Visibility("pcdestino-api"),
            Rules = new object[]
            {
                ManagedRule("CommonRules", 10, "AWSManagedRulesCommonRuleSet"),
                ManagedRule("KnownBadInputs", 20, "AWSManagedRulesKnownBadInputsRuleSet"),
                new CfnWebACL.RuleProperty
                {
                    Name = "RateLimit",
                    Priority = 30,
                    Action = new CfnWebACL.RuleActionProperty { Block = new CfnWebACL.BlockActionProperty() },
                    Statement = new CfnWebACL.StatementProperty
                    {
                        RateBasedStatement = new CfnWebACL.RateBasedStatementProperty
                        {
                            AggregateKeyType = "IP",
                            Limit = 2_000
                        }
                    },
                    VisibilityConfig = Visibility("pcdestino-rate-limit")
                }
            }
        });
        _ = new CfnWebACLAssociation(this, "WebAclAssociation", new CfnWebACLAssociationProps
        {
            ResourceArn = service.LoadBalancer.LoadBalancerArn,
            WebAclArn = webAcl.AttrArn
        });

        _ = new CfnOutput(this, "ApiUrl", new CfnOutputProps { Value = $"https://{service.LoadBalancer.LoadBalancerDnsName}" });
        _ = new CfnOutput(this, "UserPoolId", new CfnOutputProps { Value = userPool.UserPoolId });
        _ = new CfnOutput(this, "UserPoolClientId", new CfnOutputProps { Value = userPoolClient.UserPoolClientId });
        _ = new CfnOutput(this, "DatabaseSecretArn", new CfnOutputProps { Value = database.Secret!.SecretArn });
        _ = new CfnOutput(this, "EcsClusterName", new CfnOutputProps { Value = cluster.ClusterName });
        _ = new CfnOutput(this, "TaskDefinitionArn", new CfnOutputProps { Value = service.TaskDefinition.TaskDefinitionArn });
        _ = new CfnOutput(this, "ContainerName", new CfnOutputProps { Value = service.TaskDefinition.DefaultContainer!.ContainerName });
        _ = new CfnOutput(this, "ServiceSecurityGroupId", new CfnOutputProps
        {
            Value = service.Service.Connections.SecurityGroups[0].SecurityGroupId
        });
        _ = new CfnOutput(this, "ApplicationSubnetIds", new CfnOutputProps
        {
            Value = string.Join(",", vpc.SelectSubnets(new SubnetSelection { SubnetType = SubnetType.PRIVATE_WITH_EGRESS }).SubnetIds)
        });
    }

    private static CfnWebACL.RuleProperty ManagedRule(string name, double priority, string ruleGroup) => new()
    {
        Name = name,
        Priority = priority,
        OverrideAction = new CfnWebACL.OverrideActionProperty { None = new Dictionary<string, object>() },
        Statement = new CfnWebACL.StatementProperty
        {
            ManagedRuleGroupStatement = new CfnWebACL.ManagedRuleGroupStatementProperty
            {
                VendorName = "AWS",
                Name = ruleGroup
            }
        },
        VisibilityConfig = Visibility($"pcdestino-{name.ToLowerInvariant()}")
    };

    private static CfnWebACL.VisibilityConfigProperty Visibility(string name) => new()
    {
        CloudWatchMetricsEnabled = true,
        MetricName = name,
        SampledRequestsEnabled = true
    };
}
