using fcg.GameService.API.DTOs.Requests;
using fcg.GameService.API.Entities;
using fcg.GameService.API.Repositories.Interfaces;
using fcg.GameService.API.UseCases.Implementations;
using fcg.GameService.UnitTests.Utils;
using Moq;
using Shouldly;

namespace fcg.GameService.UnitTests.UseCases;

public class GameUseCaseTests
{
    private readonly Mock<IGameRepository> _repository;
    private readonly GameUseCase _useCase;

    public GameUseCaseTests()
    {
        _repository = new Mock<IGameRepository>();
        _useCase = new GameUseCase(_repository.Object);
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
            .Setup(g => g.GetByIdAsync(gameToReturn.Id))
            .ReturnsAsync(gameToReturn);

        //Act
        var result = await _useCase.GetByIdAsync(gameToReturn.Id);

        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(gameToReturn.Id);
        result.Name.ShouldBe(gameToReturn.Name);
        result.Description.ShouldBe(gameToReturn.Description);
        result.Price.ShouldBe(gameToReturn.Price);
        result.ReleasedDate.ShouldBe(gameToReturn.ReleasedDate);
        result.Tags.ShouldBe(gameToReturn.Tags);
        _repository.Verify(r => r.GetByIdAsync(gameToReturn.Id), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "GetByIdAsync_ShouldReturnNull")]
    public async Task GetByIdAsync_ShouldReturnNull()
    {
        //Arrange
        _repository
            .Setup(g => g.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Game?)null);

        //Act
        var result = await _useCase.GetByIdAsync(It.IsAny<string>());

        //Assert
        result.ShouldBeNull();
        _repository.Verify(r => r.GetByIdAsync(It.IsAny<string>()), Times.Once);
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
            .Setup(g => g.CreateAsync(game));

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
            .Setup(g => g.GetByIdAsync(game.Id))
            .ReturnsAsync(game);

        _repository
            .Setup(g => g.UpdateAsync(It.IsAny<Game>()))
            .ReturnsAsync(true);

        //Act
        var result = await _useCase.UpdateAsync(game.Id, request);

        //Assert
        result.ShouldBeTrue();
        _repository.Verify(r => r.GetByIdAsync(game.Id), Times.Once);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Game>()), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "UpdateAsync_ShouldReturnFalse")]
    public async Task UpdateAsync_ShouldReturnFalse()
    {
        //Arrange
        _repository
            .Setup(g => g.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Game?)null);

        //Act
        var result = await _useCase.UpdateAsync(It.IsAny<string>(), It.IsAny<UpdateGameDTO>());

        //Assert
        result.ShouldBeFalse();
        _repository.Verify(r => r.GetByIdAsync(It.IsAny<string>()), Times.Once);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Game>()), Times.Never);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "UpdateTagsAsync_ShouldReturnTrue")]
    public async Task UpdateTagsAsync_ShouldReturnTrue()
    {
        //Arrange
        var game = GameFaker.FakeListOfGame(1)[0];
        var request = new List<string>{
            "new-tag",
            "other-tag"
        };

        _repository
            .Setup(g => g.GetByIdAsync(game.Id))
            .ReturnsAsync(game);

        _repository
            .Setup(g => g.UpdateTagsAsync(game.Id, It.IsAny<List<string>>()))
            .ReturnsAsync(true);

        //Act
        var result = await _useCase.UpdateTagsAsync(game.Id, request);

        //Assert
        result.ShouldBeTrue();
        _repository.Verify(r => r.GetByIdAsync(game.Id), Times.Once);
        _repository.Verify(r => r.UpdateTagsAsync(game.Id, It.IsAny<List<string>>()), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "UpdateTagsAsync_ShouldReturnFalse")]
    public async Task UpdateTagsAsync_ShouldReturnFalse()
    {
        //Arrange
        var request = new List<string>{
            "new-tag",
            "other-tag"
        };

        _repository
            .Setup(g => g.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Game?)null);

        //Act
        var result = await _useCase.UpdateTagsAsync(It.IsAny<string>(), request);

        //Assert
        result.ShouldBeFalse();
        _repository.Verify(r => r.GetByIdAsync(It.IsAny<string>()), Times.Once);
        _repository.Verify(r => r.UpdateTagsAsync(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "DeleteAsync_ShouldReturnTrue")]
    public async Task DeleteAsync_ShouldReturnTrue()
    {
        //Arrange
        var game = GameFaker.FakeListOfGame(1)[0];

        _repository
            .Setup(g => g.GetByIdAsync(game.Id))
            .ReturnsAsync(game);

        _repository
            .Setup(g => g.DeleteAsync(game.Id))
            .ReturnsAsync(true);

        //Act
        var result = await _useCase.DeleteAsync(game.Id);

        //Assert
        result.ShouldBeTrue();
        _repository.Verify(r => r.GetByIdAsync(game.Id), Times.Once);
        _repository.Verify(r => r.DeleteAsync(game.Id), Times.Once);
    }

    [Trait("Module", "GameUseCase")]
    [Fact(DisplayName = "DeleteAsync_ShouldReturnFalse")]
    public async Task DeleteAsync_ShouldReturnFalse()
    {
        //Arrange
        _repository
            .Setup(g => g.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Game?)null);

        //Act
        var result = await _useCase.DeleteAsync(It.IsAny<string>());

        //Assert
        result.ShouldBeFalse();
        _repository.Verify(r => r.GetByIdAsync(It.IsAny<string>()), Times.Once);
        _repository.Verify(r => r.DeleteAsync(It.IsAny<string>()), Times.Never);
    }
}
