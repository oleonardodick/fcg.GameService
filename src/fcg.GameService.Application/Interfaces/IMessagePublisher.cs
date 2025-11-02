namespace fcg.GameService.Application.Interfaces;

public interface IMessagePublisher<TMessage> where TMessage: class
{
    Task PublishMessage(TMessage message, CancellationToken cancellationToken);
}
