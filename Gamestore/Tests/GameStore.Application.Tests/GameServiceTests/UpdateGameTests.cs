using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;
public partial class GameTests
{
    [Fact]
    public async Task UpdateSqlGameShouldUpdateGameOnce()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", "HomePage", "Description");

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName, null, null);

        List<Genre> genres = new([genre]);

        string platformName = "TestPlatform";
        Platform platform = new(Guid.NewGuid(), platformName);

        List<Platform> platforms = new([platform]);

        Guid gameId = Guid.NewGuid();
        string gameName = "TestName";
        string gameKey = "GameKey";

        Game game = new(gameId, gameName, gameKey, 5, 5, 5, null, Guid.NewGuid(), genres, platforms, new());

        _publisherRepositoryMock.Setup(x => x.Get(publisher.Id)).ReturnsAsync(publisher);
        _genreRepositoryMock.Setup(x => x.Get(genre.Id)).ReturnsAsync(genre);
        _platformRepositoryMock.Setup(x => x.Get(platform.Id)).ReturnsAsync(platform);
        _gameRepositoryMock.Setup(x => x.GetGameWithRelations(gameId)).ReturnsAsync(game);

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
                Discount = 0.2,
            },
            Platforms = new([platform.Id]),
            Genres = new([genre.Id]),
            Publisher = publisher.Id,
        };

        // Act
        await _gameService.UpdateGame(gameDto);

        // Assert
        _gameRepositoryMock.Verify(x => x.GetGameWithRelations(gameId), Times.Once);
        _publisherRepositoryMock.Verify(x => x.Get(publisher.Id), Times.Once);
        _genreRepositoryMock.Verify(x => x.Get(genre.Id), Times.Once);
        _platformRepositoryMock.Verify(x => x.Get(platform.Id), Times.Once);
        _gameRepositoryMock.Verify(x => x.Delete(It.Is<Game>(g => g.Id == gameId)), Times.Once);
        _gameRepositoryMock.Verify(x => x.Add(It.Is<Game>(g => g.Id == gameId)), Times.Once);
    }

    [Fact]
    public async Task UpdateGameIncorrectPublisherIdProvidedShouldThrowException()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", "HomePage", "Description");

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

        _publisherRepositoryMock.Setup(x => x.Get((Guid)gameDto.Publisher)).Returns(Task.FromResult<Publisher>(null));
        _genreRepositoryMock.Setup(x => x.Get(genre.Id)).ReturnsAsync(genre);
        _platformRepositoryMock.Setup(x => x.Get(platform.Id)).ReturnsAsync(platform);

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.UpdateGame(gameDto));
    }

    [Fact]
    public async Task UpdateGameIncorrectGenreIdProvidedShouldThrowException()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", null, null);

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

        _genreRepositoryMock.Setup(x => x.Get(genre.Id)).Returns(Task.FromResult<Genre>(null));
        _platformRepositoryMock.Setup(x => x.Get(platform.Id)).ReturnsAsync(platform);
        _publisherRepositoryMock.Setup(x => x.Get(publisher.Id)).ReturnsAsync(publisher);

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.UpdateGame(gameDto));
    }

    [Fact]
    public async Task UpdateGameNoPlatformIdsProvidedShouldThrowException()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", null, null);

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

        _genreRepositoryMock.Setup(x => x.Get(genre.Id)).ReturnsAsync(genre);

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.UpdateGame(gameDto));
    }

    [Fact]
    public async Task UpdateGameDescrShouldUpdateOnce()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();

        string genreName = "TestGenre";
        Genre genre = new(Guid.NewGuid(), genreName, null, null);

        List<Genre> genres = new([genre]);

        string platformName = "TestPlatform";
        Platform platform = new(Guid.NewGuid(), platformName);

        List<Platform> platforms = new([platform]);

        string gameName = "TestName";
        string gameKey = "GameKey";

        Game game = new(gameId, gameName, gameKey, 5, 5, 0.2, null, Guid.NewGuid(), genres, platforms, new());

        string updatedDescription = "Updated description";

        _gameRepositoryMock.Setup(x => x.Get(gameId)).ReturnsAsync(game);

        // Act
        await _gameService.UpdateGameDescr(gameId, updatedDescription);

        // Assert
        _gameRepositoryMock.Verify(x => x.Get(game.Id), Times.Once);
        _gameRepositoryMock.Verify(x => x.Update(It.Is<Game>(g => g.Id == gameId && g.Name == gameName && g.Key == gameKey && g.Description == updatedDescription)), Times.Once);
    }

    [Fact]
    public async Task UpdateGameDescrIncorrectGameIdProvidedShouldThrowException()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();

        string updatedDescription = "Updated description";

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _gameService.UpdateGameDescr(gameId, updatedDescription));
    }
}
