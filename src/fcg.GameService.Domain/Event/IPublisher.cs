namespace fcg.GameService.Domain.Event
{
    public interface IPublisher
    {
        Task PublishAsync<T>(T @event) where T : class;
    }
}
