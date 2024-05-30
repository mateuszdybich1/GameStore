using GameStore.Domain.Entities;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;
public partial class GameTests
{
    [Fact]
    public async Task GetAllGamesShouldReturnGameDtosList()
    {
        // Arrange
        var games = new List<Game>();

        for (int i = 1; i <= 5; ++i)
        {
            var genres = new List<Genre>();
            var platforms = new List<Platform>();
            games.Add(new Game(Guid.NewGuid(), $"Name-{i}", $"Key-{i}", i, i, i, null, Guid.NewGuid(), genres, platforms, new()));
        }

        var mongoGames = new List<Game>();

        for (int i = 5; i <= 10; ++i)
        {
            var genres = new List<Genre>();
            var platforms = new List<Platform>();
            mongoGames.Add(new Game(Guid.NewGuid(), $"Name-{i}", $"Key-{i}", i, i, i, null, Guid.NewGuid(), genres, platforms, new()));
        }

        _gameRepositoryMock.Setup(x => x.GetAllGames()).ReturnsAsync(games.AsEnumerable);
        _mongoGameRepositoryMock.Setup(x => x.GetAllGames()).ReturnsAsync(mongoGames.AsEnumerable);

        // Act
        var result = await _gameService.GetGames();
        var gameDtos = result.Games;

        // Assert
        Assert.Equal(10, gameDtos.Count);
        for (int i = 0; i < 10; ++i)
        {
            Assert.Equal($"Name-{i + 1}", gameDtos[i].Name);
            Assert.Equal($"Key-{i + 1}", gameDtos[i].Key);
        }
    }

    [Fact]
    public async Task GetGamesByPlatformIdShouldReturnGameDtosList()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        var games = new List<Game>();

        for (int i = 1; i <= 5; ++i)
        {
            var genres = new List<Genre>();
            var platforms = new List<Platform>();
            games.Add(new Game(Guid.NewGuid(), $"Name-{i}", $"Key-{i}", i, i, i, null, Guid.NewGuid(), genres, platforms, new()));
        }

        _gamesSearchCriteriaMock.Setup(x => x.GetByPlatformId(platformId)).ReturnsAsync(games.AsEnumerable);

        // Act
        var result = await _gameService.GetGamesByPlatformId(platformId);
        var gameDtos = result.ToList();

        // Assert
        Assert.Equal(5, gameDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", gameDtos[i].Name);
            Assert.Equal($"Key-{i + 1}", gameDtos[i].Key);
        }
    }

    [Fact]
    public async Task GetGamesByGenreIdShouldReturnGameDtosList()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var games = new List<Game>();

        for (int i = 1; i <= 5; ++i)
        {
            var genres = new List<Genre>();
            var platforms = new List<Platform>();
            games.Add(new Game(Guid.NewGuid(), $"Name-{i}", $"Key-{i}", i, i, i, null, Guid.NewGuid(), genres, platforms, new()));
        }

        _mongoGameRepositoryMock.Setup(x => x.GetAllGames()).ReturnsAsync(games.AsEnumerable);
        _gamesSearchCriteriaMock.Setup(x => x.GetByGenreId(genreId)).ReturnsAsync(games.AsEnumerable);

        // Act
        var result = await _gameService.GetGamesByGenreId(genreId);
        var gameDtos = result.ToList();

        // Assert
        Assert.Equal(5, gameDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", gameDtos[i].Name);
            Assert.Equal($"Key-{i + 1}", gameDtos[i].Key);
        }
    }
}
