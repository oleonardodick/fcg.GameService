using fcg.Contracts;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Presentation.DTOs.Game.Responses;
using fcg.GameService.Presentation.DTOs.GameLibrary.Responses;
using MassTransit;

namespace fcg.GameService.Infrastructure.Event.Consumers;

public class GamePurchaseConsumer : IConsumer<GamePurchaseCompleted>
{
    private readonly IAppLogger<GamePurchaseConsumer> _logger;
    private readonly IGameLibraryUseCase _gameLibrary;
    private readonly IGameUseCase _game;

    public GamePurchaseConsumer(IAppLogger<GamePurchaseConsumer> logger, IGameLibraryUseCase gameLibrary, IGameUseCase game)
    {
        _logger = logger;
        _gameLibrary = gameLibrary;
        _game = game;
    }

    public async Task Consume(ConsumeContext<GamePurchaseCompleted> context)
    {
        _logger.LogInformation("Lendo mensagem da fila game-purchase-completed");
        var message = context.Message;
        if(message is not null)
        {
            _logger.LogInformation("Mensagem encontrada: {message}", message);
            ResponseGameDTO? game = await _game.GetByIdAsync(message.GameId);
            ResponseGameLibraryDTO? library = await _gameLibrary.TryGetByUserIdAsync(message.UserId);

            if (library is null)
            {
                await _gameLibrary.CreateAsync(new()
                {
                    UserId = message.UserId,
                    Games =
                    [
                        new()
                    {
                        Id = message.GameId,
                        Name = game!.Name,
                        Tags = game!.Tags
                    }
                    ]
                });
            }
            else
            {
                await _gameLibrary.AddGameToLibraryAsync(library.Id, new()
                {
                    Id = message.GameId,
                    Name = game!.Name,
                    Tags = game!.Tags
                });
            }
        }
    }
}
