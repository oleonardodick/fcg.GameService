namespace fcg.GameService.Domain.Elasticsearch;

public interface IElasticSettings
{
    string ApiKey { get; set; }
    string CloudId { get; set; }
}