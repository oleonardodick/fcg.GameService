﻿using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Enums;
using fcg.GameService.Domain.Event;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Models;
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

        var library = await _gameLibraryUseCase.TryGetByUserIdAsync(request.UserId);
        if (library is not null)
        {
            var existsGameOnLibrary = await _gameLibraryUseCase.ExistsGameOnLibraryAsync(library.Id, request.GameId);
            if (existsGameOnLibrary) {
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

        GamePurchasePublishEvent @event = new()
        {
            UserId = request.UserId,
            GameId = request.GameId,
            Amount = game.Price,
            Currency = request.Currency.ToString(),
            PaymentMethod = ((int)request.PaymentMethod).ToString()
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
        try
        {
            GamePurchaseConsumeEvent? response = await _consumer.ConsumeAsync(cancellationToken);

            if (response is null)
            {
                _logger.LogWarning("Nenhum evento de compra para processar");
                return;
            }

            if (!response.Status.Equals(nameof(PaymentStatus.Approved), StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("Compra não aprovada, motivo: {Reason}", response.Reason!);
                return;
            }

            // Buscar jogo
            ResponseGameDTO? game = await _gameUseCase.GetByIdAsync(response.GameId);

            // Criar biblioteca do usuário se não existir
            ResponseGameLibraryDTO? library = await _gameLibraryUseCase.TryGetByUserIdAsync(response.UserId);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar evento de compra");
            throw;
        }
    }
}
