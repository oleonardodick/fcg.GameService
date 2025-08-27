using fcg.GameService.Domain.Models;

namespace fcg.GameService.Domain.Elasticsearch;

public interface IElasticClient<T>
{
    Task<IReadOnlyCollection<T>> Get(ElasticLogRequest elasticLogRequest);

    Task<bool> AddOrUpdate(T log, string index);
}
