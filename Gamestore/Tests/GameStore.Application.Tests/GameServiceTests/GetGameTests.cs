using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;
public class GetGameTests
{
    private readonly GameService _gameService;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IGamesSearchCriteria> _gamesSearchCriteriaMock;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IPublisherRepository> _publisherRepositoryMock;

    public GetGameTests()
    {
        _gameRepositoryMock = new();
        _gamesSearchCriteriaMock = new();
        _platformRepositoryMock = new();
        _genreRepositoryMock = new();
        _publisherRepositoryMock = new();

        _gameService = new(_gameRepositoryMock.Object, _gamesSearchCriteriaMock.Object, _platformRepositoryMock.Object, _genreRepositoryMock.Object, _publisherRepositoryMock.Object);
    }

    [Fact]
    public void GetByGameKeyShouldReturnGameDto()
    {
        // Arrange
        string gameKey = "TestKey";

        var platforms = new List<Platform>();
        var genres = new List<Genre>();

        var game = new Game(Guid.NewGuid(), "TestName", gameKey, 5, 5, 5, Guid.NewGuid(), genres, platforms);
        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns(game);

        // Act
        GameDto gameDto = _gameService.GetGameByKey(gameKey);

        // Assert
        Assert.NotNull(gameDto);
        Assert.Equal(gameKey, gameDto.Key);
        _gamesSearchCriteriaMock.Verify(x => x.GetByKey(gameKey), Times.Once);
    }

    [Fact]
    public void GetByGameKeyIncorrectKeyProvidedShouldThrowException()
    {
        // Arrange
        string gameKey = "TestKey";
        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns((Game)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.GetGameByKey(gameKey));
    }

    [Fact]
    public void GetByGameIdShouldReturnGameDto()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();

        var platforms = new List<Platform>();
        var genres = new List<Genre>();

        var game = new Game(gameId, "TestName", "TestKey", 5, 5, 5, Guid.NewGuid(), genres, platforms);
        _gameRepositoryMock.Setup(x => x.GetGame(gameId)).Returns(game);

        // Act
        GameDto gameDto = _gameService.GetGameById(gameId);

        // Assert
        Assert.NotNull(gameDto);
        Assert.Equal(gameId, gameDto.Id);
        _gameRepositoryMock.Verify(x => x.GetGame(gameId), Times.Once);
    }

    [Fact]
    public void GetByGameIdIncorrectKeyProvidedShouldThrowException()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        _gameRepositoryMock.Setup(x => x.GetGame(gameId)).Returns((Game)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.GetGameById(gameId));
    }

    [Fact]
    public void GetGameByKeyWithRelationsShouldReturnCorrectObject()
    {
        // Arrange
        string gameKey = "TestKey";

        var genres = new List<Genre> { new(Guid.NewGuid(), "Genre1"), new(Guid.NewGuid(), "Genre2") };
        var platforms = new List<Platform> { new(Guid.NewGuid(), "type1"), new(Guid.NewGuid(), "type2") };
        var publisher = new Publisher(Guid.NewGuid(), "TestCompany", string.Empty, string.Empty);

        var game = new Game(Guid.NewGuid(), "Test Name", gameKey, 5, 5, 5, Guid.NewGuid(), genres, platforms);

        _gamesSearchCriteriaMock.Setup(x => x.GetByKeyWithRelations(gameKey)).Returns(game);

        // Act
        var result = _gameService.GetGameByKeyWithRelations(gameKey);

        // Assert
        Assert.NotNull(result);
        _gamesSearchCriteriaMock.Verify(x => x.GetByKeyWithRelations(gameKey), Times.Once);
    }
}
