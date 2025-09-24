using fcg.GameService.Domain.Elasticsearch;

namespace fcg.GameService.Infrastructure.Configurations;

public class ElasticSettings : IElasticSettings
{
    public string ApiKey { get; set; } = null!;
    public string CloudId { get; set; } = null!;
}
