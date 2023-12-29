using GameStore.Application.Dtos;
using GameStore.Application.Exceptions;
using GameStore.Application.Services;
using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;

public class AddGameTests
{
    private readonly GameService _gameService;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IGamesSearchCriteria> _gamesSearchCriteriaMock;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;

    public AddGameTests()
    {
        _gameRepositoryMock = new();
        _gamesSearchCriteriaMock = new();
        _platformRepositoryMock = new();
        _genreRepositoryMock = new();

        _gameService = new(_gameRepositoryMock.Object, _gamesSearchCriteriaMock.Object, _platformRepositoryMock.Object, _genreRepositoryMock.Object);
    }

    [Fact]
    public void AddGameShouldAddGameOnce()
    {
        // Arrange
        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName);

        string platformName = "TestPlatform";
        Platform platform = new(Guid.NewGuid(), platformName);

        Guid gameId = Guid.NewGuid();
        string gameName = "TestGame";
        string gameKey = "TestKey";

        GameDto gameDto = new()
        {
            Name = gameName,
            Key = gameKey,
            PlatformsIds = new([platform.Id]),
            GenresIds = new([genre.Id]),
        };

        _platformRepositoryMock.Setup(x => x.GetPlatform(platform.Id)).Returns(platform);
        _genreRepositoryMock.Setup(x => x.GetGenre(genre.Id)).Returns(genre);

        // Act
        var gameReturnedId = _gameService.AddGame(gameDto);

        // Assert
        _gameRepositoryMock.Verify(x => x.AddGame(It.IsNotNull<Game>()), Times.Once());
        Assert.True(gameReturnedId != Guid.Empty);
    }

    [Fact]
    public void AddGameIncorrectGenreIdProvidedShouldThrowException()
    {
        // Arrange
        string platformName = "TestPlatform";
        Platform platform = new(Guid.NewGuid(), platformName);

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName);

        string gameName = "TestGame";
        string gameKey = "TestKey";

        GameDto gameDto = new()
        {
            Name = gameName,
            Key = gameKey,
            PlatformsIds = new([platform.Id]),
            GenresIds = new([genre.Id]),
        };

        _genreRepositoryMock.Setup(x => x.GetGenre(genre.Id)).Returns((Genre)null);
        _platformRepositoryMock.Setup(x => x.GetPlatform(platform.Id)).Returns(platform);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.AddGame(gameDto));
    }

    [Fact]
    public void AddGameNoPlatformIdsProvidedShouldThrowException()
    {
        // Arrange
        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName);

        string gameName = "TestGame";
        string gameKey = "TestKey";

        GameDto gameDto = new()
        {
            Name = gameName,
            Key = gameKey,
            GenresIds = new([genre.Id]),
        };

        _genreRepositoryMock.Setup(x => x.GetGenre(genre.Id)).Returns(genre);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.AddGame(gameDto));
    }
}
