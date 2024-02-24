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

        var game = new Game(Guid.NewGuid(), "TestName", gameKey, 5, 5, 5, Guid.NewGuid(), genres, platforms);
        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).ReturnsAsync(game);

        // Act
        GameDto gameDto = await _gameService.GetGameByKey(gameKey);

        // Assert
        Assert.NotNull(gameDto);
        Assert.Equal(gameKey, gameDto.Key);
        _gamesSearchCriteriaMock.Verify(x => x.GetByKey(gameKey), Times.Once);
    }

    [Fact]
    public async Task GetByGameKeyIncorrectKeyProvidedShouldThrowException()
    {
        // Arrange
        string gameKey = "TestKey";
        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns(Task.FromResult<Game>(null));

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

        var game = new Game(gameId, "TestName", "TestKey", 5, 5, 5, Guid.NewGuid(), genres, platforms);
        _gameRepositoryMock.Setup(x => x.Get(gameId)).ReturnsAsync(game);

        // Act
        GameDto gameDto = await _gameService.GetGameById(gameId);

        // Assert
        Assert.NotNull(gameDto);
        Assert.Equal(gameId, gameDto.Id);
        _gameRepositoryMock.Verify(x => x.Get(gameId), Times.Once);
    }

    [Fact]
    public async Task GetByGameIdIncorrectKeyProvidedShouldThrowException()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        _gameRepositoryMock.Setup(x => x.Get(gameId)).Returns(Task.FromResult<Game>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.GetGameById(gameId));
    }

    [Fact]
    public async Task GetGameByKeyWithRelationsShouldReturnCorrectObject()
    {
        // Arrange
        string gameKey = "TestKey";

        var genres = new List<Genre> { new(Guid.NewGuid(), "Genre1"), new(Guid.NewGuid(), "Genre2") };
        var platforms = new List<Platform> { new(Guid.NewGuid(), "type1"), new(Guid.NewGuid(), "type2") };
        var publisher = new Publisher(Guid.NewGuid(), "TestCompany", string.Empty, string.Empty);

        var game = new Game(Guid.NewGuid(), "Test Name", gameKey, 5, 5, 5, Guid.NewGuid(), genres, platforms);

        _gamesSearchCriteriaMock.Setup(x => x.GetByKeyWithRelations(gameKey)).ReturnsAsync(game);

        // Act
        var result = await _gameService.GetGameByKeyWithRelations(gameKey);

        // Assert
        Assert.NotNull(result);
        _gamesSearchCriteriaMock.Verify(x => x.GetByKeyWithRelations(gameKey), Times.Once);
    }
}
