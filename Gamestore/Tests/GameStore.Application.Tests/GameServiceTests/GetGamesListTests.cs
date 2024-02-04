using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;
public partial class GameTests
{
    [Fact]
    public void GetAllGamesShouldReturnGameDtosList()
    {
        // Arrange
        var games = new List<Game>();

        for (int i = 1; i <= 5; ++i)
        {
            var genres = new List<Genre>();
            var platforms = new List<Platform>();
            games.Add(new Game(Guid.NewGuid(), $"Name-{i}", $"Key-{i}", i, i, i, Guid.NewGuid(), genres, platforms));
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
            games.Add(new Game(Guid.NewGuid(), $"Name-{i}", $"Key-{i}", i, i, i, Guid.NewGuid(), genres, platforms));
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
            games.Add(new Game(Guid.NewGuid(), $"Name-{i}", $"Key-{i}", i, i, i, Guid.NewGuid(), genres, platforms));
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
