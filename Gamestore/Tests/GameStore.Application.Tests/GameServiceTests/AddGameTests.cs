using GameStore.Application.Dtos;
using GameStore.Application.Services;
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
    private readonly Mock<IGamesSearchCriteria> _gamesSearchCriteriaMock;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IPublisherRepository> _publisherRepositoryMock;

    public GameTests()
    {
        _gameRepositoryMock = new();
        _gamesSearchCriteriaMock = new();
        _platformRepositoryMock = new();
        _genreRepositoryMock = new();
        _publisherRepositoryMock = new();

        _gameService = new(_gameRepositoryMock.Object, _gamesSearchCriteriaMock.Object, _platformRepositoryMock.Object, _genreRepositoryMock.Object, _publisherRepositoryMock.Object);
    }

    [Fact]
    public async Task AddGameShouldAddGameOnce()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", "TestHomePage", "TestDescription");

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName);

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
                Discount = 1,
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
                Discount = 1,
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
                Discount = 1,
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
        Genre genre = new(Guid.NewGuid(), genreName);

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
                Discount = 1,
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
