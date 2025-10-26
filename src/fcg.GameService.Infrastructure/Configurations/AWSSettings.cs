using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace fcg.GameService.Infrastructure.Configurations;

public class AWSSettings
{
    public string ServiceURL { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public SQS SQS { get; set; } = default!;
}

public class SQS
{
    public string GamePurchaseRequested { get; set; } = string.Empty;
    public string GamePurchaseCompleted { get; set; } = string.Empty;
}

public static class AWSService
{
    public static void SetAWSSettingsFromConfiguration(IConfiguration configuration)
    {
        AWSSettings awsSettings = new();
        configuration.GetSection(nameof(AWSSettings)).Bind(awsSettings);

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_ACCESS_KEY")))
            if (string.IsNullOrEmpty(awsSettings.AccessKey))
                throw new InvalidCredentialException("Parâmetro Access Key não configurada");

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_SECRET_KEY")))
            if (string.IsNullOrEmpty(awsSettings.SecretKey))
                throw new InvalidCredentialException("Parâmetro Secret Key não configurada");

        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY", awsSettings.AccessKey);
        Environment.SetEnvironmentVariable("AWS_SECRET_KEY", awsSettings.SecretKey);

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN")))
            if (!string.IsNullOrEmpty(awsSettings.Token))
                Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", awsSettings.Token);
    }
}
