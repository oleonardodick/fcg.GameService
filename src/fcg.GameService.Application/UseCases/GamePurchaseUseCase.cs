using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Enums;
using fcg.GameService.Domain.Event;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;
using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using fcg.GameService.Presentation.DTOs.GameLibrary.Responses;
using fcg.GameService.Presentation.Event.Consume;
using fcg.GameService.Presentation.Event.Publish;

namespace fcg.GameService.Application.UseCases;

public class GamePurchaseUseCase(
    IGameUseCase gameUseCase,
    IGameLibraryUseCase gameLibraryUseCase,
    IPublisher<GamePurchasePublishEvent> publisher,
    IConsumer<GamePurchaseConsumeEvent> consumer) : IPurchaseUseCase
{
    private readonly IGameUseCase _gameUseCase = gameUseCase;
    private readonly IGameLibraryUseCase _gameLibraryUseCase = gameLibraryUseCase;
    private readonly IPublisher<GamePurchasePublishEvent> _publisher = publisher;
    private readonly IConsumer<GamePurchaseConsumeEvent> _consumer = consumer;
    private const string ENTITY = "Jogo";

    public async Task<ResponseQueuedDto> PublishAsync(PurchaseGameDTO request, CancellationToken cancellationToken)
    {
        ResponseGameDTO game = await _gameUseCase.GetByIdAsync(request.GameId) ??
            throw AppNotFoundException.ForEntity(ENTITY, request.GameId);

        GamePurchasePublishEvent @event = new()
        {
            UserId = request.UserId,
            GameId = request.GameId,
            Amount = game.Price,
            Currency = nameof(request.Currency),
            PaymentMethod = nameof(request.PaymentMethod)
        };

        await _publisher.PublishAsync(@event, cancellationToken);

        return new ResponseQueuedDto
        {
            Message = "Pedido de compra realizado com sucesso",
            PaymentId = @event.PaymentId,
            Status = QueueStatus.Queued,
            CorrelationId = @event.CorrelationId
        };
    }

    public async Task<ConsumedQueueDto> ConsumeAsync(CancellationToken cancellationToken)
    {
        GamePurchaseConsumeEvent? @event = await _consumer.ConsumeAsync(cancellationToken);

        if (@event is null)
            return new ConsumedQueueDto { Status = QueueStatus.Failed };

        // Buscar jogo
        ResponseGameDTO? game = await _gameUseCase.GetByIdAsync(@event.GameId);

        // Criar biblioteca do usuário se não existir
        ResponseGameLibraryDTO? library = await _gameLibraryUseCase.GetByUserIdAsync(@event.UserId);

        if (library is null)
        {
            await _gameLibraryUseCase.CreateAsync(new()
            {
                UserId = @event.UserId,
                Games =
                [
                    new()
                    {
                        Id = @event.GameId,
                        Name = game!.Name,
                        Tags = game!.Tags
                    }
                ]
            });
        }
        else
        {
            // Adicionar jogo na biblioteca do usuário
            await _gameLibraryUseCase.AddGameToLibraryAsync(library.Id, new()
            {
                Id = @event.GameId,
                Name = game!.Name,
                Tags = game!.Tags
            });
        }

        return new ConsumedQueueDto
        {
            Status = QueueStatus.Consumed
        };
    }
}
