using GameStore.Application.Exceptions;
using GameStore.Application.Services;
using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;
public class DeleteGameTests
{
    private readonly GameService _gameService;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IGamesSearchCriteria> _gamesSearchCriteriaMock;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;

    public DeleteGameTests()
    {
        _gameRepositoryMock = new();
        _gamesSearchCriteriaMock = new();
        _platformRepositoryMock = new();
        _genreRepositoryMock = new();

        _gameService = new(_gameRepositoryMock.Object, _gamesSearchCriteriaMock.Object, _platformRepositoryMock.Object, _genreRepositoryMock.Object);
    }

    [Fact]
    public void DeleteGameShouldDeleteGameOnce()
    {
        // Arrange
        string gameKey = "GameKey";

        Game game = new(Guid.NewGuid(), "Name", gameKey, new([new()]), new([new()]));
        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns(game);

        // Act
        _gameService.DeleteGame(gameKey);

        // Assert
        _gamesSearchCriteriaMock.Verify(x => x.GetByKey(gameKey), Times.Once);
        _gameRepositoryMock.Verify(x => x.RemoveGame(It.Is<Game>(g => g == game)), Times.Once);
    }

    [Fact]
    public void DeleteGameIncorrectGameKeyProvidedShouldTHrowException()
    {
        // Arrange
        string gameKey = "GameKey";

        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns((Game)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.DeleteGame(gameKey));
    }
}
