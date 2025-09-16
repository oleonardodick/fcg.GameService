namespace fcg.GameService.Domain.Event
{
    public interface IPublisher<T> where T : class
    {
        Task PublishAsync(T message, CancellationToken cancellationToken);
    }
}
