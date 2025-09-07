using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Entities;
using fcg.GameService.Domain.Enums;
using fcg.GameService.Domain.Event;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;
using fcg.GameService.Presentation.Event.Consume;
using fcg.GameService.Presentation.Event.Publish;

namespace fcg.GameService.Application.UseCases;

public class PurchaseUseCase(
    IGameRepository gameRepository,
    IPublisher publisher,
    IConsumer consumer) : IPurchaseUseCase
{
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IPublisher _publisher = publisher;
    private readonly IConsumer _consumer = consumer;
    private const string ENTITY = "Jogo";

    public async Task<ResponseQueuedDto> PublishAsync(PurchaseGameDTO request)
    {
        Game? game = await _gameRepository.GetByIdAsync(request.GameId) ??
            throw AppNotFoundException.ForEntity(ENTITY, request.GameId);

        GamePurchasePublishEvent @event = new()
        {
            UserId = request.UserId,
            GameId = request.GameId,
            Amount = game.Price,
            Currency = nameof(request.Currency),
            PaymentMethod = nameof(request.PaymentMethod)
        };

        await _publisher.PublishAsync(@event);

        return new ResponseQueuedDto
        {
            IsQueued = true,
            Message = "Pedido de compra realizado com sucesso",
            PaymentId = @event.PaymentId,
            Status = QueueStatus.Queued
        };
    }

    public async Task<ConsumedQueueDto?> ConsumeAsync()
    {
        GamePurchaseConsumeEvent? @event = await _consumer.ConsumeAsync<GamePurchaseConsumeEvent>();

        if (@event is null)
            return null;

        return new ConsumedQueueDto
        {
            PaymentId = @event.PaymentId,
            UserId = @event.UserId,
            GameId = @event.GameId,
            CorrelationId = @event.CorrelationId
        };
    }
}
