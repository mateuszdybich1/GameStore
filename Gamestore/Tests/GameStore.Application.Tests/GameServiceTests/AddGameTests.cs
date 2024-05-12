using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;

public partial class GameTests
{
    private readonly GameService _gameService;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IGameRepository> _mongoGameRepositoryMock;
    private readonly Mock<IGamesSearchCriteria> _gamesSearchCriteriaMock;
    private readonly Mock<IGamesSearchCriteria> _mongoGamesSearchCriteriaMock;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IGenreRepository> _mongoGenreRepositoryMock;
    private readonly Mock<IPublisherRepository> _publisherRepositoryMock;
    private readonly Mock<IPublisherRepository> _mongoPublisherRepositoryMock;
    private readonly Mock<IChangeLogService> _changeLogServiceMock;
    private readonly Mock<IFakeDataGenerator> _fakeDataGeneratorMock;

    public GameTests()
    {
        _gameRepositoryMock = new();
        _mongoGameRepositoryMock = new();
        Mock<Func<RepositoryTypes, IGameRepository>> mockGameRepositoryFactory = new MockRepositoryFactory<IGameRepository>().GetGamesRepository(_gameRepositoryMock, _mongoGameRepositoryMock);

        _gamesSearchCriteriaMock = new();
        _mongoGamesSearchCriteriaMock = new();
        Mock<Func<RepositoryTypes, IGamesSearchCriteria>> mockGameSearchCriteriaRepositoryFactory = new MockRepositoryFactory<IGamesSearchCriteria>().GetGamesRepository(_gamesSearchCriteriaMock, _mongoGamesSearchCriteriaMock);

        _platformRepositoryMock = new();

        _genreRepositoryMock = new();
        _mongoGenreRepositoryMock = new();
        Mock<Func<RepositoryTypes, IGenreRepository>> mockGenreRepositoryFactory = new MockRepositoryFactory<IGenreRepository>().GetGamesRepository(_genreRepositoryMock, _mongoGenreRepositoryMock);

        _publisherRepositoryMock = new();
        _mongoPublisherRepositoryMock = new();
        Mock<Func<RepositoryTypes, IPublisherRepository>> mockPublisherRepositoryFactory = new MockRepositoryFactory<IPublisherRepository>().GetGamesRepository(_publisherRepositoryMock, _mongoPublisherRepositoryMock);

        _changeLogServiceMock = new();

        _fakeDataGeneratorMock = new();

        _gameService = new(mockGameRepositoryFactory.Object, mockGameSearchCriteriaRepositoryFactory.Object, _platformRepositoryMock.Object, mockGenreRepositoryFactory.Object, mockPublisherRepositoryFactory.Object, _changeLogServiceMock.Object, _fakeDataGeneratorMock.Object);
    }

    [Fact]
    public async Task AddGameShouldAddGameOnce()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", "TestHomePage", "TestDescription");

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName, null, null);

        string platformName = "TestPlatform";
        Platform platform = new(Guid.NewGuid(), platformName);

        Guid gameId = Guid.NewGuid();
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
                Discount = 0.2,
            },
            Platforms = new([platform.Id]),
            Genres = new([genre.Id]),
            Publisher = publisher.Id,
        };

        _publisherRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(publisher);
        _platformRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(platform);
        _genreRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(genre);

        // Act
        var gameReturnedId = await _gameService.AddGame(gameDto);

        // Assert
        Assert.True(gameReturnedId != Guid.Empty);
        _gameRepositoryMock.Verify(x => x.Add(It.Is<Game>(x => x.Id == gameReturnedId && x.Name == gameDto.Game.Name && x.Key == gameDto.Game.Key && x.Price == gameDto.Game.Price && x.UnitInStock == gameDto.Game.UnitInStock && x.Discount == gameDto.Game.Discount && x.PublisherId == gameDto.Publisher && x.Platforms.Select(platform => platform.Id).SequenceEqual(gameDto.Platforms) && x.Genres.Select(genre => genre.Id).SequenceEqual(gameDto.Genres))), Times.Once());
    }

    [Fact]
    public async Task AddGameIncorrectGenreIdProvidedShouldThrowException()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", "TestHomePage", "TestDescription");

        string platformName = "TestPlatform";
        Platform platform = new(Guid.NewGuid(), platformName);

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName, null, null);

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
                Discount = 0.2,
            },
            Platforms = new([platform.Id]),
            Genres = new([genre.Id]),
            Publisher = publisher.Id,
        };

        _publisherRepositoryMock.Setup(x => x.Get((Guid)gameDto.Publisher)).ReturnsAsync(publisher);
        _genreRepositoryMock.Setup(x => x.Get(genre.Id)).Returns(Task.FromResult<Genre>(null));
        _platformRepositoryMock.Setup(x => x.Get(platform.Id)).ReturnsAsync(platform);

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.AddGame(gameDto));
    }

    [Fact]
    public async Task AddGameIncorrectPlatformIdsProvidedShouldThrowException()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", "TestHomePage", "TestDescription");

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName, null, null);

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
                Discount = 0.2,
            },
            Genres = new([genre.Id]),
            Publisher = publisher.Id,
        };

        _publisherRepositoryMock.Setup(x => x.Get((Guid)gameDto.Publisher)).ReturnsAsync(publisher);
        _genreRepositoryMock.Setup(x => x.Get(genre.Id)).ReturnsAsync(genre);

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.AddGame(gameDto));
    }

    [Fact]
    public async Task AddGameIncorrectPublisherIdProvidedShouldThrowException()
    {
        // Arrange
        string platformName = "TestPlatform";
        Platform platform = new(Guid.NewGuid(), platformName);

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName, null, null);

        string gameName = "TestGame";
        string gameKey = "TestKey";

        Guid publisherId = Guid.NewGuid();

        GameDtoDto gameDto = new()
        {
            Game = new()
            {
                Name = gameName,
                Key = gameKey,
                Price = 3.14,
                UnitInStock = 5,
                Discount = 0.2,
            },
            Platforms = new([platform.Id]),
            Genres = new([genre.Id]),
            Publisher = publisherId,
        };

        _publisherRepositoryMock.Setup(x => x.Get((Guid)gameDto.Publisher)).Returns(Task.FromResult<Publisher>(null));
        _genreRepositoryMock.Setup(x => x.Get(genre.Id)).ReturnsAsync(genre);
        _platformRepositoryMock.Setup(x => x.Get(platform.Id)).ReturnsAsync(platform);

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.AddGame(gameDto));
    }
}
