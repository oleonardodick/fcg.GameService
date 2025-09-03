using fcg.GameService.Application.Interfaces;
using fcg.GameService.Application.UseCases;
using fcg.GameService.Domain.Elasticsearch;
using fcg.GameService.Domain.Entities;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.UnitTests.Fixtures;
using fcg.GameService.UnitTests.Utils;
using MongoDB.Bson;
using Moq;
using Shouldly;

namespace fcg.GameService.UnitTests.UseCases;

public class GameUseCaseTests : IClassFixture<MappingFixture>
{
    private readonly Mock<IGameRepository> _repository;
    private readonly Mock<IElasticClient<GameLog>> _elasticMock;
    private readonly Mock<IAppLogger<GameUseCase>> _loggerMock;
    private readonly GameUseCase _useCase;
    private const string ENTITY = "Jogo";

    public GameUseCaseTests()
    {
        _repository = new Mock<IGameRepository>();
        _elasticMock = new Mock<IElasticClient<GameLog>>();
        _loggerMock = new Mock<IAppLogger<GameUseCase>>();
        _useCase = new GameUseCase(_repository.Object, _elasticMock.Object, _loggerMock.Object);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "GetAllAsync_ShouldReturnAllGames")]
    public async Task GetAllAsync_ShouldReturnAllGames()
    {
        //Arrange
        var games = GameFaker.FakeListOfGame(10);
        
        _repository
            .Setup(g => g.GetAllAsync())
            .ReturnsAsync(games);

        //Act
        var result = await _useCase.GetAllAsync();

        //Assert
        result.ShouldNotBeEmpty();
        result.Count().ShouldBe(games.Count());
        _repository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "GetAllAsync_ShouldReturnEmptyList")]
    public async Task GetAllAsync_ShouldReturnEmptyList()
    {
        //Arrange
        _repository
            .Setup(g => g.GetAllAsync())
            .ReturnsAsync([]);

        //Act
        var result = await _useCase.GetAllAsync();

        //Assert
        result.ShouldBeEmpty();
        _repository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "GetByIdAsync_ShouldReturnAGameByItsId")]
    public async Task GetByIdAsync_ShouldReturnAGameByItsId()
    {
        //Arrange
        var games = GameFaker.FakeListOfGame(10);
        var gameToReturn = games[5];

        _repository
            .Setup(g => g.GetByIdAsync(gameToReturn.Id!))
            .ReturnsAsync(gameToReturn);

        //Act
        var result = await _useCase.GetByIdAsync(gameToReturn.Id!);

        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(gameToReturn.Id);
        result.Name.ShouldBe(gameToReturn.Name);
        result.Description.ShouldBe(gameToReturn.Description);
        result.Price.ShouldBe(gameToReturn.Price);
        result.ReleasedDate.ShouldBe(gameToReturn.ReleasedDate);
        result.Tags.ShouldBe(gameToReturn.Tags);
        _repository.Verify(r => r.GetByIdAsync(gameToReturn.Id!), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "GetByIdAsync_ShouldThrowGameNotFound")]
    public async Task GetByIdAsync_ShouldThrowGameNotFound()
    {
        //Arrange
        var id = ObjectId.GenerateNewId().ToString();

        _repository
            .Setup(g => g.GetByIdAsync(id))
            .ReturnsAsync((Game?)null);

        //Act & Assert
        var exception = await Should.ThrowAsync<AppNotFoundException>(() => _useCase.GetByIdAsync(id));
        exception.Message.ShouldBe($"{ENTITY} não encontrado com o ID {id}");
        _repository.Verify(g => g.GetByIdAsync(id), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "CreateAsync_ShouldCreateAGame")]
    public async Task CreateAsync_ShouldCreateAGame()
    {
        //Arrange
        var game = GameFaker.FakeListOfGame(1)[0];

        var request = new CreateGameDTO
        {
            Name = game.Name,
            Description = game.Description,
            Tags = game.Tags,
            Price = game.Price,
            ReleasedDate = game.ReleasedDate
        };

        _repository
            .Setup(g => g.CreateAsync(It.IsAny<Game>()))
            .ReturnsAsync(game);

        //Act
        var result = await _useCase.CreateAsync(request);

        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBeNullOrWhiteSpace();
        result.Name.ShouldBe(request.Name);
        result.Description.ShouldBe(request.Description);
        result.Price.ShouldBe(request.Price);
        result.ReleasedDate.ShouldBe(request.ReleasedDate);
        result.Tags.ShouldBe(request.Tags);
        _repository.Verify(r => r.CreateAsync(It.IsAny<Game>()), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "UpdateAsync_ShouldReturnTrue")]
    public async Task UpdateAsync_ShouldReturnTrue()
    {
        //Arrange
        var game = GameFaker.FakeListOfGame(1)[0];
        var request = new UpdateGameDTO
        {
            Name = "New name"
        };

        _repository
            .Setup(g => g.GetByIdAsync(game.Id!))
            .ReturnsAsync(game);

        _repository
            .Setup(g => g.UpdateAsync(It.IsAny<Game>()))
            .ReturnsAsync(true);

        //Act
        var result = await _useCase.UpdateAsync(game.Id!, request);

        //Assert
        result.ShouldBeTrue();
        _repository.Verify(r => r.GetByIdAsync(game.Id!), Times.Once);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Game>()), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "UpdateAsync_ShouldThrowGameNotFound")]
    public async Task UpdateAsync_ShouldThrowGameNotFound()
    {
        //Arrange
        var id = ObjectId.GenerateNewId().ToString();

        _repository
            .Setup(g => g.GetByIdAsync(id))
            .ReturnsAsync((Game?)null);

        //Act & Assert
        var exception = await Should.ThrowAsync<AppNotFoundException>(() => _useCase.GetByIdAsync(id));
        exception.Message.ShouldBe($"{ENTITY} não encontrado com o ID {id}");
        _repository.Verify(g => g.GetByIdAsync(id), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "UpdateTagsAsync_ShouldReturnTrue")]
    public async Task UpdateTagsAsync_ShouldReturnTrue()
    {
        //Arrange
        var game = GameFaker.FakeListOfGame(1)[0];
        var request = new UpdateTagsDTO {
            Tags = ["new-tag", "other-tag"]
        };

        _repository
            .Setup(g => g.GetByIdAsync(game.Id!))
            .ReturnsAsync(game);

        _repository
            .Setup(g => g.UpdateTagsAsync(game.Id!, It.IsAny<List<string>>()))
            .ReturnsAsync(true);

        //Act
        var result = await _useCase.UpdateTagsAsync(game.Id!, request);

        //Assert
        result.ShouldBeTrue();
        _repository.Verify(r => r.GetByIdAsync(game.Id!), Times.Once);
        _repository.Verify(r => r.UpdateTagsAsync(game.Id!, It.IsAny<List<string>>()), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "UpdateTagsAsync_ShouldThrowGameNotFound")]
    public async Task UpdateTagsAsync_ShouldThrowGameNotFound()
    {
        //Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var request = new UpdateTagsDTO {
            Tags = ["new-tag", "other-tag"]
        };

        _repository
            .Setup(g => g.GetByIdAsync(id))
            .ReturnsAsync((Game?)null);


        //Act & Assert
        var exception = await Should.ThrowAsync<AppNotFoundException>(() => _useCase.GetByIdAsync(id));
        exception.Message.ShouldBe($"{ENTITY} não encontrado com o ID {id}");
        _repository.Verify(g => g.GetByIdAsync(id), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "DeleteAsync_ShouldReturnTrue")]
    public async Task DeleteAsync_ShouldReturnTrue()
    {
        //Arrange
        var game = GameFaker.FakeListOfGame(1)[0];

        _repository
            .Setup(g => g.GetByIdAsync(game.Id!))
            .ReturnsAsync(game);

        _repository
            .Setup(g => g.DeleteAsync(game.Id!))
            .ReturnsAsync(true);

        //Act
        var result = await _useCase.DeleteAsync(game.Id!);

        //Assert
        result.ShouldBeTrue();
        _repository.Verify(r => r.GetByIdAsync(game.Id!), Times.Once);
        _repository.Verify(r => r.DeleteAsync(game.Id!), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "DeleteAsync_ShouldThrowGameNotFound")]
    public async Task DeleteAsync_ShouldThrowGameNotFound()
    {
        //Arrange
        var id = ObjectId.GenerateNewId().ToString();

        _repository
            .Setup(g => g.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Game?)null);


        //Act & Assert
        var exception = await Should.ThrowAsync<AppNotFoundException>(() => _useCase.GetByIdAsync(id));
        exception.Message.ShouldBe($"{ENTITY} não encontrado com o ID {id}");
        _repository.Verify(g => g.GetByIdAsync(id), Times.Once);
    }
}
