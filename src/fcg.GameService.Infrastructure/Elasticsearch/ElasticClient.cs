using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using fcg.GameService.Domain.Elasticsearch;
using fcg.GameService.Domain.Models;

namespace fcg.GameService.Infrastructure.Elasticsearch;

public class ElasticClient<T>(IElasticSettings settings) : IElasticClient<T>
{
    private readonly ElasticsearchClient _client = new(settings.CloudId, new ApiKey(settings.ApiKey));

    public async Task<IReadOnlyCollection<T>> Get(ElasticLogRequest elasticLogRequest)
    {
        SearchRequest request = new(elasticLogRequest.Index.ToLowerInvariant())
        {
            From = elasticLogRequest.Page,
            Size = elasticLogRequest.Size,
            Query = new MatchQuery(elasticLogRequest.Field, elasticLogRequest.Value)
        };

        SearchResponse<T> response = await _client.SearchAsync<T>(request);

        if (response.IsValidResponse)
        {
            return response.Documents;
        }

        return [];
    }

    public async Task<bool> AddOrUpdate(T log, string index)
    {
        IndexResponse response = await _client.IndexAsync(log, x => x.Index(index.ToLowerInvariant()));

        if (response.IsValidResponse)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
