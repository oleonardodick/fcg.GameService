using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Enums;
using fcg.GameService.Domain.Event;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;
using fcg.GameService.Presentation.DTOs.GameLibrary.Responses;
using fcg.GameService.Presentation.Event.Consume;
using fcg.GameService.Presentation.Event.Publish;

namespace fcg.GameService.Application.UseCases;

public class GamePurchaseUseCase(
    IGameUseCase _gameUseCase,
    IGameLibraryUseCase _gameLibraryUseCase,
    IPublisher<GamePurchasePublishEvent> _publisher,
    IConsumer<GamePurchaseConsumeEvent> _consumer,
    IAppLogger<GamePurchaseUseCase> _logger) : IPurchaseUseCase
{
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

    public async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        GamePurchaseConsumeEvent? response = await _consumer.ConsumeAsync(cancellationToken);

        if (response is null)
        {
            _logger.LogWarning("Nenhum evento de compra para processar");
            return;
        }

        if (response.Status != nameof(PaymentStatus.PaymentApproved))
        {
            _logger.LogWarning("Evento de compra com status inválido: {Status}", response.Status);
            return;
        }

        // Buscar jogo
        ResponseGameDTO? game = await _gameUseCase.GetByIdAsync(response.GameId);

        // Criar biblioteca do usuário se não existir
        ResponseGameLibraryDTO? library = await _gameLibraryUseCase.GetByUserIdAsync(response.UserId);

        if (library is null)
        {
            await _gameLibraryUseCase.CreateAsync(new()
            {
                UserId = response.UserId,
                Games =
                [
                    new()
                    {
                        Id = response.GameId,
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
                Id = response.GameId,
                Name = game!.Name,
                Tags = game!.Tags
            });
        }
    }
}
