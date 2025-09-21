namespace fcg.GameService.Infrastructure.Configurations;

public static class ElasticSettings
{
    public static string ApiKey { get; } =
        Environment.GetEnvironmentVariable("ElasticSettings_ApiKey")!;
    public static string CloudId { get; } =
        Environment.GetEnvironmentVariable("ElasticSettings_CloudId")!;
}
