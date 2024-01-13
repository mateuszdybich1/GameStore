using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;
public class GetGamesListTests
{
    private readonly GameService _gameService;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IGamesSearchCriteria> _gamesSearchCriteriaMock;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;

    public GetGamesListTests()
    {
        _gameRepositoryMock = new();
        _gamesSearchCriteriaMock = new();
        _platformRepositoryMock = new();
        _genreRepositoryMock = new();

        _gameService = new(_gameRepositoryMock.Object, _gamesSearchCriteriaMock.Object, _platformRepositoryMock.Object, _genreRepositoryMock.Object);
    }

    [Fact]
    public void GetAllGamesShouldReturnGameDtosList()
    {
        // Arrange
        var games = new List<Game>();

        for (int i = 1; i <= 5; ++i)
        {
            var genres = new List<Genre>();
            var platforms = new List<Platform>();
            games.Add(new Game(Guid.NewGuid(), $"Name-{i}", $"Key-{i}", genres, platforms));
        }

        _gameRepositoryMock.Setup(x => x.GetAllGames()).Returns(games);

        // Act
        List<GameDto> gameDtos = _gameService.GetGames();

        // Assert
        Assert.Equal(5, gameDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", gameDtos[i].Name);
            Assert.Equal($"Key-{i + 1}", gameDtos[i].Key);
        }
    }

    [Fact]
    public void GetGamesByPlatformIdShouldReturnGameDtosList()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        var games = new List<Game>();

        for (int i = 1; i <= 5; ++i)
        {
            var genres = new List<Genre>();
            var platforms = new List<Platform>();
            games.Add(new Game(Guid.NewGuid(), $"Name-{i}", $"Key-{i}", genres, platforms));
        }

        _gamesSearchCriteriaMock.Setup(x => x.GetByPlatformId(platformId)).Returns(games);

        // Act
        List<GameDto> gameDtos = _gameService.GetGamesByPlatformId(platformId);

        // Assert
        Assert.Equal(5, gameDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", gameDtos[i].Name);
            Assert.Equal($"Key-{i + 1}", gameDtos[i].Key);
        }
    }

    [Fact]
    public void GetGamesByGenreIdShouldReturnGameDtosList()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var games = new List<Game>();

        for (int i = 1; i <= 5; ++i)
        {
            var genres = new List<Genre>();
            var platforms = new List<Platform>();
            games.Add(new Game(Guid.NewGuid(), $"Name-{i}", $"Key-{i}", genres, platforms));
        }

        _gamesSearchCriteriaMock.Setup(x => x.GetByGenreId(genreId)).Returns(games);

        // Act
        List<GameDto> gameDtos = _gameService.GetGamesByGenreId(genreId);

        // Assert
        Assert.Equal(5, gameDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", gameDtos[i].Name);
            Assert.Equal($"Key-{i + 1}", gameDtos[i].Key);
        }
    }
}
