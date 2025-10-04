using fcg.GameService.Application.Interfaces;
using fcg.GameService.Application.Mappers.Adapters;
using fcg.GameService.Application.UseCases;
using fcg.GameService.Domain.Elasticsearch;
using fcg.GameService.Domain.Entities;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using fcg.GameService.UnitTests.Fixtures;
using fcg.GameService.UnitTests.Utils;
using MongoDB.Bson;
using Moq;
using Shouldly;

namespace fcg.GameService.UnitTests.UseCases;

public class GameLibraryUseCaseTests : IClassFixture<MappingFixture>
{
    private readonly Mock<IGameLibraryRepository> _repository;
    private readonly Mock<IElasticClient<UserLog>> _elasticMock;
    private readonly Mock<IAppLogger<GameLibraryUseCase>> _loggerMock;
    private readonly GameLibraryUseCase _useCase;
    private const string ENTITY = "Biblioteca";
    private readonly Random random = new();

    public GameLibraryUseCaseTests()
    {
        _repository = new Mock<IGameLibraryRepository>();
        _elasticMock = new Mock<IElasticClient<UserLog>>();
        _loggerMock = new Mock<IAppLogger<GameLibraryUseCase>>();
        _useCase = new GameLibraryUseCase(_repository.Object, _elasticMock.Object, _loggerMock.Object);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "GetByIdAsync_ShouldReturnAGameLibraryByItsId")]
    public async Task GetByIdAsync_ShouldReturnAGameLibraryByItsId()
    {
        //Arrange
        var gameLibraries = GameLibraryFaker.FakeListOfGameLibrary(5, random.Next(1,5));
        var gameLibrary = gameLibraries[2];

        _repository
            .Setup(g => g.GetByIdAsync(gameLibrary.Id!))
            .ReturnsAsync(gameLibrary);

        //Act
        var result = await _useCase.GetByIdAsync(gameLibrary.Id!);

        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(gameLibrary.Id);
        result.UserId.ShouldBe(gameLibrary.UserId);
        result.Games.Select(g => new { g.Id, g.Name })
            .ShouldBe(gameLibrary.Games.Select(g => new { g.Id, g.Name }));
        _repository.Verify(g => g.GetByIdAsync(gameLibrary.Id!), Times.Once);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "GetByIdAsync_ShouldThrowGameLibraryNotFound")]
    public async Task GetByIdAsync_ShouldThrowGameLibraryNotFound()
    {
        //Arrange
        var id = ObjectId.GenerateNewId().ToString();
        _repository
            .Setup(g => g.GetByIdAsync(id))
            .ReturnsAsync((GameLibrary?)null);

        //Act & Assert
        var exception = await Should.ThrowAsync<AppNotFoundException>(() => _useCase.GetByIdAsync(id));
        exception.Message.ShouldBe($"{ENTITY} não encontrado com o ID {id}");
        _repository.Verify(g => g.GetByIdAsync(It.IsAny<string>()), Times.Once);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "GetByUserIdAsync_ShouldReturnTheLibraryFromUser")]
    public async Task GetByUserIdAsync_ShouldReturnTheLibraryFromUser()
    {
        //Arrange
        var gameLibraries = GameLibraryFaker.FakeListOfGameLibrary(5, random.Next(1,5));
        var gameLibrary = gameLibraries[4];

        _repository
            .Setup(g => g.GetByUserIdAsync(gameLibrary.UserId))
            .ReturnsAsync(gameLibrary);

        //Act
        var result = await _useCase.GetByUserIdAsync(gameLibrary.UserId);

        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(gameLibrary.Id);
        result.UserId.ShouldBe(gameLibrary.UserId);
        result.Games.Select(g => new { g.Id, g.Name })
            .ShouldBe(gameLibrary.Games.Select(g => new { g.Id, g.Name }));
        _repository.Verify(g => g.GetByUserIdAsync(gameLibrary.UserId), Times.Once);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "GetByUserIdAsync_ShouldThrowGameLibraryNotFound_ByUserId")]
    public async Task GetByUserIdAsync_ShouldReturnNull()
    {
        //Arrange
        var userId = Guid.NewGuid().ToString();

        _repository
            .Setup(g => g.GetByUserIdAsync(userId))
            .ReturnsAsync((GameLibrary?)null);

        //Act & Assert
        var exception = await Should.ThrowAsync<AppNotFoundException>(() => _useCase.GetByUserIdAsync(userId));
        exception.Message.ShouldBe($"{ENTITY} não encontrada para o usuário {userId}");
        _repository.Verify(g => g.GetByUserIdAsync(It.IsAny<string>()), Times.Once);        
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "TryGetByUserIdAsync_ShouldReturnTheLibraryFromUser")]
    public async Task TryGetByUserIdAsync_ShouldReturnTheLibraryFromUser()
    {
        //Arrange
        var gameLibraries = GameLibraryFaker.FakeListOfGameLibrary(5, random.Next(1,5));
        var gameLibrary = gameLibraries[4];

        _repository
            .Setup(g => g.GetByUserIdAsync(gameLibrary.UserId))
            .ReturnsAsync(gameLibrary);

        //Act
        var result = await _useCase.TryGetByUserIdAsync(gameLibrary.UserId);

        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(gameLibrary.Id);
        result.UserId.ShouldBe(gameLibrary.UserId);
        result.Games.Select(g => new { g.Id, g.Name })
            .ShouldBe(gameLibrary.Games.Select(g => new { g.Id, g.Name }));
        _repository.Verify(g => g.GetByUserIdAsync(gameLibrary.UserId), Times.Once);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "TryGetByUserIdAsync_ShouldReturnNull")]
    public async Task TryGetByUserIdAsync_ShouldReturnNull()
    {
        //Arrange
        var userId = Guid.NewGuid().ToString();

        _repository
            .Setup(g => g.GetByUserIdAsync(userId))
            .ReturnsAsync((GameLibrary?)null);

        //Act
        var result = await _useCase.TryGetByUserIdAsync(userId);

        //Assert
        result.ShouldBeNull();
        _repository.Verify(g => g.GetByUserIdAsync(userId), Times.Once);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "CreateAsync_ShouldCreateTheLibrary")]
    public async Task CreateAsync_ShouldCreateTheLibrary()
    {
        //Arrange
        var gameLibrary = GameLibraryFaker.FakeListOfGameLibrary(1, 0)[0];

        var request = new CreateGameLibraryDTO
        {
            UserId = Guid.NewGuid().ToString()
        };

        _repository
            .Setup(g => g.CreateAsync(It.IsAny<GameLibrary>()))
            .ReturnsAsync(gameLibrary);

        //Act
        var result = await _useCase.CreateAsync(request);

        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBeNullOrWhiteSpace();
        result.UserId.ShouldBe(gameLibrary.UserId);
        result.Games.ShouldBeEmpty();
        _repository.Verify(g => g.CreateAsync(It.IsAny<GameLibrary>()), Times.Once);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "AddGameToLibraryAsync_ShouldReturnTrue")]
    public async Task AddGameToLibraryAsync_ShouldReturnTrue()
    {
        //Arrange
        var gameLibrary = GameLibraryFaker.FakeListOfGameLibrary(1, random.Next(1,5))[0];
        var gameToAdd = GameLibraryFaker.FakeListOfGameAdquired(1)[0];

        var request = new AddGameToLibraryDTO
        {
            Id = gameToAdd.Id,
            Name = gameToAdd.Name
        };

        _repository
            .Setup(g => g.GetByIdAsync(gameLibrary.Id!))
            .ReturnsAsync(gameLibrary);

        _repository
            .Setup(g => g.AddGameToLibraryAsync(It.IsAny<string>(), It.IsAny<GameAdquired>()))
            .ReturnsAsync(true);

        //Act
        var result = await _useCase.AddGameToLibraryAsync(gameLibrary.Id!, request);

        //Assert
        result.ShouldBeTrue();
        _repository.Verify(g => g.GetByIdAsync(It.IsAny<string>()), Times.Once);
        _repository.Verify(g => g.AddGameToLibraryAsync(It.IsAny<string>(), It.IsAny<GameAdquired>()), Times.Once);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "AddGameToLibraryAsync_ShouldThrowLibraryNotFound")]
    public async Task AddGameToLibraryAsync_ShouldThrowLibraryNotFound()
    {
        //Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var request = new AddGameToLibraryDTO
        {
            Id = It.IsAny<string>(),
            Name = It.IsAny<string>()
        };

        _repository
            .Setup(g => g.GetByIdAsync(id))
            .ReturnsAsync((GameLibrary?)null);

        //Act & Assert
        var exception = await Should.ThrowAsync<AppNotFoundException>(() => _useCase.GetByIdAsync(id));
        exception.Message.ShouldBe($"{ENTITY} não encontrado com o ID {id}");
        _repository.Verify(g => g.GetByIdAsync(It.IsAny<string>()), Times.Once);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "RemoveGameFromLibraryAsync_ShouldReturnTrue")]
    public async Task RemoveGameFromLibraryAsync_ShouldReturnTrue()
    {
        //Arrange
        var gameLibrary = GameLibraryFaker.FakeListOfGameLibrary(1, random.Next(1,5))[0];

        var request = new RemoveGameFromLibraryDTO
        {
            Id = It.IsAny<string>(),
        };

        _repository
            .Setup(g => g.GetByIdAsync(gameLibrary.Id!))
            .ReturnsAsync(gameLibrary);

        _repository
            .Setup(g => g.RemoveGameFromLibraryAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        //Act
        var result = await _useCase.RemoveGameFromLibraryAsync(gameLibrary.Id!, request);

        //Assert
        result.ShouldBeTrue();
        _repository.Verify(g => g.GetByIdAsync(It.IsAny<string>()), Times.Once);
        _repository.Verify(g => g.RemoveGameFromLibraryAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "RemoveGameFromLibraryAsync_ShouldThrowLibraryNotFound")]
    public async Task RemoveGameFromLibraryAsync_ShouldThrowLibraryNotFound()
    {
        //Arrange
        var id = ObjectId.GenerateNewId().ToString();

        var request = new RemoveGameFromLibraryDTO
        {
            Id = It.IsAny<string>(),
        };

        _repository
            .Setup(g => g.GetByIdAsync(id))
            .ReturnsAsync((GameLibrary?)null);

        //Act & Assert
        var exception = await Should.ThrowAsync<AppNotFoundException>(() => _useCase.GetByIdAsync(id));
        exception.Message.ShouldBe($"{ENTITY} não encontrado com o ID {id}");
        _repository.Verify(g => g.GetByIdAsync(It.IsAny<string>()), Times.Once);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "ExistsGameOnLibraryAsync_ShouldReturnTrue")]
    public async Task ExistsGameOnLibraryAsync_ShouldReturnTrue()
    {
        //Arrange
        _repository
            .Setup(g => g.ExistsGameOnLibraryAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        //Act
        var result = await _useCase.ExistsGameOnLibraryAsync(It.IsAny<string>(), It.IsAny<string>());

        //Assert
        result.ShouldBeTrue();
        _repository.Verify(g => g.ExistsGameOnLibraryAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Trait("Module", "GameLibraryUseCase")]
    [Fact(DisplayName = "ExistsGameOnLibraryAsync_ShouldReturnFalse")]
    public async Task ExistsGameOnLibraryAsync_ShouldReturnFalse()
    {
        //Arrange
        _repository
            .Setup(g => g.ExistsGameOnLibraryAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        //Act
        var result = await _useCase.ExistsGameOnLibraryAsync(It.IsAny<string>(), It.IsAny<string>());

        //Assert
        result.ShouldBeFalse();
        _repository.Verify(g => g.ExistsGameOnLibraryAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}
