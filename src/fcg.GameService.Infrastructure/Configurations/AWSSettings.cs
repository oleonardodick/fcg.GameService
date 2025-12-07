using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class AWSSettings
{
    public string ServiceURL { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public SQS SQS { get; set; } = new();
}

public class SQS
{
    public string GamePurchaseRequested { get; set; } = string.Empty;
    public string GamePurchaseCompleted { get; set; } = string.Empty;
}

public static class AWSService
{
    public static AWSSettings LoadAndValidate(IConfiguration configuration)
    {
        var settings = new AWSSettings();
        configuration.GetSection(nameof(AWSSettings)).Bind(settings);

        // Busca variáveis de ambiente primeiro
        var envAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        var envSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        var envSessionToken = Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN");

        // -----------------------------
        // ACCESS KEY
        // -----------------------------
        if (string.IsNullOrWhiteSpace(envAccessKey) && string.IsNullOrWhiteSpace(settings.AccessKey))
            throw new InvalidCredentialException("AWS AccessKey não configurada (AWS_ACCESS_KEY_ID ou AWSSettings:AccessKey).");

        // Caso só exista no appsettings, seta no ambiente
        if (string.IsNullOrWhiteSpace(envAccessKey) && !string.IsNullOrWhiteSpace(settings.AccessKey))
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", settings.AccessKey);

        // -----------------------------
        // SECRET KEY
        // -----------------------------
        if (string.IsNullOrWhiteSpace(envSecretKey) && string.IsNullOrWhiteSpace(settings.SecretKey))
            throw new InvalidCredentialException("AWS SecretKey não configurada (AWS_SECRET_ACCESS_KEY ou AWSSettings:SecretKey).");

        if (string.IsNullOrWhiteSpace(envSecretKey) && !string.IsNullOrWhiteSpace(settings.SecretKey))
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", settings.SecretKey);

        // -----------------------------
        // SESSION TOKEN (opcional)
        // -----------------------------
        if (string.IsNullOrWhiteSpace(envSessionToken) && !string.IsNullOrWhiteSpace(settings.Token))
            Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", settings.Token);

        // -----------------------------
        // REGION
        // -----------------------------
        if (string.IsNullOrWhiteSpace(settings.Region))
            throw new InvalidOperationException("AWS Region não configurada (AWSSettings:Region).");

        // -----------------------------
        // SQS QUEUES
        // -----------------------------
        if (string.IsNullOrWhiteSpace(settings.SQS?.GamePurchaseRequested))
            throw new InvalidOperationException("Fila SQS GamePurchaseRequested não configurada (AWSSettings:SQS:GamePurchaseRequested).");

        if (string.IsNullOrWhiteSpace(settings.SQS?.GamePurchaseCompleted))
            throw new InvalidOperationException("Fila SQS GamePurchaseCompleted não configurada (AWSSettings:SQS:GamePurchaseCompleted).");

        return settings;
    }

    public static void AddAWSSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var validated = LoadAndValidate(configuration);
        services.AddSingleton(validated);
    }
}
