using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
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
    private readonly Mock<IPublisherRepository> _publisherRepositoryMock;

    public DeleteGameTests()
    {
        _gameRepositoryMock = new();
        _gamesSearchCriteriaMock = new();
        _platformRepositoryMock = new();
        _genreRepositoryMock = new();
        _publisherRepositoryMock = new();

        _gameService = new(_gameRepositoryMock.Object, _gamesSearchCriteriaMock.Object, _platformRepositoryMock.Object, _genreRepositoryMock.Object, _publisherRepositoryMock.Object);
    }

    [Fact]
    public void DeleteGameShouldDeleteGameOnce()
    {
        // Arrange
        string gameKey = "GameKey";

        Game game = new(Guid.NewGuid(), "Name", gameKey, 5, 5, 5, Guid.NewGuid(), new([new()]), new([new()]));
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
