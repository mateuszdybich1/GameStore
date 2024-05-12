using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;
public partial class GameTests
{
    [Fact]
    public async Task GetByGameKeyShouldReturnGameDto()
    {
        // Arrange
        string gameKey = "TestKey";

        var platforms = new List<Platform>();
        var genres = new List<Genre>();

        var game = new Game(Guid.NewGuid(), "TestName", gameKey, 5, 5, 5, null, Guid.NewGuid(), genres, platforms, new());
        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).ReturnsAsync(game);
        _mongoGamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns(Task.FromResult<Game>(null));

        // Act
        GameDto gameDto = await _gameService.GetGameByKey(gameKey);

        // Assert
        Assert.NotNull(gameDto);
        Assert.Equal(gameKey, gameDto.Key);
        _gamesSearchCriteriaMock.Verify(x => x.GetByKey(gameKey), Times.Once);
    }

    [Fact]
    public async Task GetMongoGameByGameKeyShouldReturnGameDto()
    {
        // Arrange
        string gameKey = "TestKey";

        var platforms = new List<Platform>();
        var genres = new List<Genre>();

        var game = new Game(Guid.NewGuid(), "TestName", gameKey, 5, 5, 5, null, Guid.NewGuid(), genres, platforms, new());
        _mongoGamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).ReturnsAsync(game);
        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns(Task.FromResult<Game>(null));

        // Act
        GameDto gameDto = await _gameService.GetGameByKey(gameKey);

        // Assert
        Assert.NotNull(gameDto);
        Assert.Equal(gameKey, gameDto.Key);
        _mongoGamesSearchCriteriaMock.Verify(x => x.GetByKey(gameKey), Times.Once);
    }

    [Fact]
    public async Task GetByGameKeyIncorrectKeyProvidedShouldThrowException()
    {
        // Arrange
        string gameKey = "TestKey";
        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns(Task.FromResult<Game>(null));
        _mongoGamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns(Task.FromResult<Game>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.GetGameByKey(gameKey));
    }

    [Fact]
    public async Task GetByGameIdShouldReturnGameDto()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();

        var platforms = new List<Platform>();
        var genres = new List<Genre>();

        var game = new Game(gameId, "TestName", "TestKey", 5, 5, 5, null, Guid.NewGuid(), genres, platforms, new());
        _gameRepositoryMock.Setup(x => x.Get(gameId)).ReturnsAsync(game);
        _mongoGameRepositoryMock.Setup(x => x.Get(gameId)).Returns(Task.FromResult<Game>(null));

        // Act
        GameDto gameDto = await _gameService.GetGameById(gameId);

        // Assert
        Assert.NotNull(gameDto);
        Assert.Equal(gameId, gameDto.Id);
        _gameRepositoryMock.Verify(x => x.Get(gameId), Times.Once);
    }

    [Fact]
    public async Task GetByGameIdIncorrectIdProvidedShouldThrowException()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        _gameRepositoryMock.Setup(x => x.Get(gameId)).Returns(Task.FromResult<Game>(null));
        _mongoGameRepositoryMock.Setup(x => x.Get(gameId)).Returns(Task.FromResult<Game>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.GetGameById(gameId));
    }

    [Fact]
    public async Task GetGameByKeyWithRelationsShouldReturnObject()
    {
        // Arrange
        string gameKey = "TestKey";

        var genres = new List<Genre> { new(Guid.NewGuid(), "Genre1", null, null), new(Guid.NewGuid(), "Genre2", null, null) };
        var platforms = new List<Platform> { new(Guid.NewGuid(), "type1"), new(Guid.NewGuid(), "type2") };
        var publisher = new Publisher(Guid.NewGuid(), "TestCompany", string.Empty, string.Empty);

        var game = new Game(Guid.NewGuid(), "Test Name", gameKey, 5, 5, 5, null, Guid.NewGuid(), genres, platforms, new());

        _gamesSearchCriteriaMock.Setup(x => x.GetByKeyWithRelations(gameKey)).ReturnsAsync(game);
        _mongoGamesSearchCriteriaMock.Setup(x => x.GetByKeyWithRelations(gameKey)).Returns(Task.FromResult<object>(null));

        // Act
        var result = await _gameService.GetGameByKeyWithRelations(gameKey);

        // Assert
        Assert.NotNull(result);
        _gamesSearchCriteriaMock.Verify(x => x.GetByKeyWithRelations(gameKey), Times.Once);
    }
}
