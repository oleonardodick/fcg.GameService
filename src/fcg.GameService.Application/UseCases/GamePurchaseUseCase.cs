using fcg.Contracts;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Enums;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Models;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.Game.Responses;

namespace fcg.GameService.Application.UseCases;

public class GamePurchaseUseCase(
    IGameUseCase _gameUseCase,
    IGameLibraryUseCase _gameLibraryUseCase,
    IMessagePublisher<GamePurchaseRequested> _publisher) : IPurchaseUseCase
{
    private const string ENTITY = "Jogo";

    public async Task<ResponseQueuedDto> PublishAsync(PurchaseGameDTO request, CancellationToken cancellationToken)
    {
        ResponseGameDTO game = await _gameUseCase.GetByIdAsync(request.GameId) ??
            throw AppNotFoundException.ForEntity(ENTITY, request.GameId);

        var library = await _gameLibraryUseCase.TryGetByUserIdAsync(request.UserId);
        if (library is not null)
        {
            var existsGameOnLibrary = await _gameLibraryUseCase.ExistsGameOnLibraryAsync(library.Id, request.GameId);
            if (existsGameOnLibrary)
            {
                List<ErrorDetails> error = [
                        new ErrorDetails {
                            Property = nameof(request.GameId),
                            Errors = ["Jogo já cadastrado na biblioteca do usuário."]
                        }
                    ];

                throw new AppValidationException(
                    error
                );
            }
        }
        
        GamePurchaseRequested message = new()
        {
            PaymentId = Guid.NewGuid(),
            UserId = request.UserId,
            GameId = request.GameId,
            Amount = game.Price,
            Currency = request.Currency.ToString(),
            PaymentMethod = ((int)request.PaymentMethod).ToString(),
            CorrelationId = Guid.NewGuid(),
        };

        await _publisher.PublishMessage(message, cancellationToken);

        return new ResponseQueuedDto
        {
            Message = "Pedido de compra realizado com sucesso",
            PaymentId = message.PaymentId,
            Status = QueueStatus.Queued,
            CorrelationId = message.CorrelationId
        };
    }
}
