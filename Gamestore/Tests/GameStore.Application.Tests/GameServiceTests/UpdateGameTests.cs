using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;
public class UpdateGameTests
{
    private readonly GameService _gameService;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IGamesSearchCriteria> _gamesSearchCriteriaMock;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IPublisherRepository> _publisherRepositoryMock;

    public UpdateGameTests()
    {
        _gameRepositoryMock = new();
        _gamesSearchCriteriaMock = new();
        _platformRepositoryMock = new();
        _genreRepositoryMock = new();
        _publisherRepositoryMock = new();

        _gameService = new(_gameRepositoryMock.Object, _gamesSearchCriteriaMock.Object, _platformRepositoryMock.Object, _genreRepositoryMock.Object, _publisherRepositoryMock.Object);
    }

    [Fact]
    public void UpdateGameShouldUpdateGameOnce()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", "HomePage", "Description");

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName);

        List<Genre> genres = new([genre]);

        string platformName = "TestPlatform";
        Platform platform = new(Guid.NewGuid(), platformName);

        List<Platform> platforms = new([platform]);

        Guid gameId = Guid.NewGuid();
        string gameName = "TestName";
        string gameKey = "GameKey";

        Game game = new(gameId, gameName, gameKey, 5, 5, 5, Guid.NewGuid(), genres, platforms);

        _publisherRepositoryMock.Setup(x => x.GetPublisher(publisher.Id)).Returns(publisher);
        _genreRepositoryMock.Setup(x => x.GetGenre(genre.Id)).Returns(genre);
        _platformRepositoryMock.Setup(x => x.GetPlatform(platform.Id)).Returns(platform);
        _gameRepositoryMock.Setup(x => x.GetGameWithRelations(gameId)).Returns(game);

        string updatedName = "updatedName";
        string updatedKey = "updatedKey";

        GameDtoDto gameDto = new()
        {
            Game = new()
            {
                Id = gameId,
                Name = updatedName,
                Key = updatedKey,
                Price = 3.14,
                UnitInStock = 5,
                Discontinued = 10,
            },
            Platforms = new([platform.Id]),
            Genres = new([genre.Id]),
            Publisher = publisher.Id,
        };

        // Act
        _gameService.UpdateGame(gameDto);

        // Assert
        _gameRepositoryMock.Verify(x => x.GetGameWithRelations(gameId), Times.Once);
        _publisherRepositoryMock.Verify(x => x.GetPublisher(publisher.Id), Times.Once);
        _genreRepositoryMock.Verify(x => x.GetGenre(genre.Id), Times.Once);
        _platformRepositoryMock.Verify(x => x.GetPlatform(platform.Id), Times.Once);
        _gameRepositoryMock.Verify(x => x.UpdateGame(It.Is<Game>(g => g.Id == gameId && g.Name == updatedName && g.Key == updatedKey && g.Genres.Select(genre => genre.Id).SequenceEqual(gameDto.Genres) && g.Platforms.Select(platform => platform.Id).SequenceEqual(gameDto.Platforms))), Times.Once);
    }

    [Fact]
    public void UpdateGameIncorrectPublisherIdProvidedShouldThrowException()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", "HomePage", "Description");

        string platformName = "TestPlatform";
        Platform platform = new(Guid.NewGuid(), platformName);

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName);

        string gameName = "TestGame";
        string gameKey = "TestKey";

        GameDtoDto gameDto = new()
        {
            Game = new()
            {
                Name = gameName,
                Key = gameKey,
                Price = 3.14,
                UnitInStock = 5,
                Discontinued = 10,
            },
            Platforms = new([platform.Id]),
            Genres = new([genre.Id]),
            Publisher = publisher.Id,
        };

        _publisherRepositoryMock.Setup(x => x.GetPublisher((Guid)gameDto.Publisher)).Returns((Publisher)null);
        _genreRepositoryMock.Setup(x => x.GetGenre(genre.Id)).Returns(genre);
        _platformRepositoryMock.Setup(x => x.GetPlatform(platform.Id)).Returns(platform);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.UpdateGame(gameDto));
    }

    [Fact]
    public void UpdateGameIncorrectGenreIdProvidedShouldThrowException()
    {
        // Arrange
        string platformName = "TestPlatform";
        Platform platform = new(Guid.NewGuid(), platformName);

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName);

        string gameName = "TestGame";
        string gameKey = "TestKey";

        GameDtoDto gameDto = new()
        {
            Game = new()
            {
                Name = gameName,
                Key = gameKey,
                Price = 3.14,
                UnitInStock = 5,
                Discontinued = 10,
            },
            Platforms = new([platform.Id]),
            Genres = new([genre.Id]),
        };

        _genreRepositoryMock.Setup(x => x.GetGenre(genre.Id)).Returns((Genre)null);
        _platformRepositoryMock.Setup(x => x.GetPlatform(platform.Id)).Returns(platform);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.UpdateGame(gameDto));
    }

    [Fact]
    public void UpdateGameNoPlatformIdsProvidedShouldThrowException()
    {
        // Arrange
        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName);

        string gameName = "TestGame";
        string gameKey = "TestKey";

        GameDtoDto gameDto = new()
        {
            Game = new()
            {
                Name = gameName,
                Key = gameKey,
                Price = 3.14,
                UnitInStock = 5,
                Discontinued = 10,
            },
            Genres = new([genre.Id]),
        };

        _genreRepositoryMock.Setup(x => x.GetGenre(genre.Id)).Returns(genre);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.UpdateGame(gameDto));
    }

    [Fact]
    public void UpdateGameDescrShouldUpdateOnce()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName);

        List<Genre> genres = new([genre]);

        string platformName = "TestPlatform";
        Platform platform = new(Guid.NewGuid(), platformName);

        List<Platform> platforms = new([platform]);

        string gameName = "TestName";
        string gameKey = "GameKey";

        Game game = new(gameId, gameName, gameKey, 5, 5, 5, Guid.NewGuid(), genres, platforms);

        string updatedDescription = "Updated description";

        _gameRepositoryMock.Setup(x => x.GetGame(gameId)).Returns(game);

        // Act
        _gameService.UpdateGameDescr(gameId, updatedDescription);

        // Assert
        _gameRepositoryMock.Verify(x => x.GetGame(game.Id), Times.Once);
        _gameRepositoryMock.Verify(x => x.UpdateGame(It.Is<Game>(g => g.Id == gameId && g.Name == gameName && g.Key == gameKey && g.Description == updatedDescription)), Times.Once);
    }

    [Fact]
    public void UpdateGameDescrIncorrectGameIdProvidedShouldThrowException()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();

        string updatedDescription = "Updated description";

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.UpdateGameDescr(gameId, updatedDescription));
    }
}
