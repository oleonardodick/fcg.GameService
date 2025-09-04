using fcg.GameService.Application.Interfaces;
using fcg.GameService.Application.UseCases;
using fcg.GameService.Domain.Elasticsearch;
using fcg.GameService.Domain.Entities;
using fcg.GameService.Domain.Models;
using fcg.GameService.Domain.Repositories;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.UnitTests.Fixtures;
using fcg.GameService.UnitTests.Utils;
using Moq;
using Shouldly;

namespace fcg.GameService.UnitTests.UseCases;

public class SuggestionUseCaseTests : IClassFixture<MappingFixture>
{
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IElasticClient<GameLog>> _gameElasticMock;

    private readonly Mock<IGameLibraryRepository> _libraryRepositoryMock;
    private readonly Mock<IElasticClient<UserLog>> _userElasticMock;

    private readonly Mock<IAppLogger<SuggestionUseCase>> _loggerMock;

    private readonly SuggestionUseCase _useCase;

    public SuggestionUseCaseTests()
    {
        _gameRepositoryMock = new Mock<IGameRepository>();
        _gameElasticMock = new Mock<IElasticClient<GameLog>>();
        _libraryRepositoryMock = new Mock<IGameLibraryRepository>();
        _userElasticMock = new Mock<IElasticClient<UserLog>>();
        _loggerMock = new Mock<IAppLogger<SuggestionUseCase>>();
        _useCase = new SuggestionUseCase(
            _gameRepositoryMock.Object,
            _libraryRepositoryMock.Object,
            _userElasticMock.Object,
            _gameElasticMock.Object,
            _loggerMock.Object);
    }

    [Trait("Module", "SuggestionUseCase")]
    [Fact(DisplayName = "GetSuggestionByUserIdAsync_WithoutGames_ShouldReturnAllGames")]
    public async Task GetSuggestionByUserIdAsync_WithoutGames_ShouldReturnAllGames()
    {
        //Arrange
        var games = GameFaker.FakeListOfGame(10);
        var library = GameLibraryFaker.FakeListOfGameLibrary(1, 0);

        var request = new SuggestGameDto
        {
            UserId = library.First().UserId,
            Page = 1,
            Size = 10
        };

        _gameRepositoryMock
            .Setup(g => g.GetAllByIdsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(games);

        _libraryRepositoryMock
            .Setup(g => g.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(library.First());

        _userElasticMock
            .Setup(e => e.Get(It.IsAny<ElasticLogRequest>()))
            .ReturnsAsync(UserLogFaker.FakeListOfUserLog(library, games, 1));

        _gameElasticMock
            .Setup(e => e.Get(It.IsAny<ElasticLogRequest>()))
            .ReturnsAsync(GameLogFaker.FakeListOfGameLog(games));

        //Act
        var result = await _useCase.GetSuggestionByUserIdAsync(request);

        //Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(games.Count);
    }

    [Trait("Module", "SuggestionUseCase")]
    [Fact(DisplayName = "GetSuggestionByUserIdAsync_WithoutUserLog_ShouldReturnEmptyList")]
    public async Task GetSuggestionByUserIdAsync_WithoutUserLog_ShouldReturnEmptyList()
    {
        //Arrange
        var games = GameFaker.FakeListOfGame(10);
        var library = GameLibraryFaker.FakeListOfGameLibrary(1, 0);

        var request = new SuggestGameDto
        {
            UserId = library.First().UserId,
            Page = 1,
            Size = 10
        };

        _userElasticMock
            .Setup(e => e.Get(It.IsAny<ElasticLogRequest>()))
            .ReturnsAsync([]);

        //Act
        var result = await _useCase.GetSuggestionByUserIdAsync(request);

        //Assert
        result.ShouldBeEmpty();
        _loggerMock.Verify(l => l.LogWarning("Erro ao consultar Biblioteca no Elasticsearch"), Times.Once);
    }

    [Trait("Module", "SuggestionUseCase")]
    [Fact(DisplayName = "GetSuggestionByUserIdAsync_WithGames_ShouldReturnSomeGames")]
    public async Task GetSuggestionByUserIdAsync_WithGames_ShouldReturnSomeGames()
    {
        //Arrange
        var games = GameFaker.FakeListOfGame(10);
        var library = GameLibraryFaker.FakeListOfGameLibrary(1, 4, [.. games.Skip(6)]);

        var request = new SuggestGameDto
        {
            UserId = library.First().UserId,
            Page = 1,
            Size = 10
        };

        var expectedGames = games.Take(6).ToList();

        _gameRepositoryMock
            .Setup(g => g.GetAllByIdsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(expectedGames);

        _libraryRepositoryMock
            .Setup(g => g.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(library.First());

        _userElasticMock
            .Setup(e => e.Get(It.IsAny<ElasticLogRequest>()))
            .ReturnsAsync(UserLogFaker.FakeListOfUserLog(library, games, 1));

        _gameElasticMock
            .Setup(e => e.Get(It.IsAny<ElasticLogRequest>()))
            .ReturnsAsync(GameLogFaker.FakeListOfGameLog(games));

        //Act
        var result = await _useCase.GetSuggestionByUserIdAsync(request);

        //Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(6);
    }

    [Trait("Module", "SuggestionUseCase")]
    [Fact(DisplayName = "GetSuggestionByUserIdAsync_WithoutGameLog_ShouldReturnEmptyList")]
    public async Task GetSuggestionByUserIdAsync_WithoutGameLog_ShouldReturnEmptyList()
    {
        //Arrange
        var games = GameFaker.FakeListOfGame(10);
        var library = GameLibraryFaker.FakeListOfGameLibrary(1, 0);

        var request = new SuggestGameDto
        {
            UserId = library.First().UserId,
            Page = 1,
            Size = 10
        };

        _userElasticMock
            .Setup(e => e.Get(It.IsAny<ElasticLogRequest>()))
            .ReturnsAsync(UserLogFaker.FakeListOfUserLog(library, games, 1));

        _gameElasticMock
            .Setup(e => e.Get(It.IsAny<ElasticLogRequest>()))
            .ReturnsAsync([]);

        //Act
        var result = await _useCase.GetSuggestionByUserIdAsync(request);

        //Assert
        result.ShouldBeEmpty();
        _loggerMock.Verify(l => l.LogWarning("Erro ao consultar Jogo no Elasticsearch"), Times.Once);
    }
}
