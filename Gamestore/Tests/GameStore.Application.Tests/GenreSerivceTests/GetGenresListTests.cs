using GameStore.Domain.Entities;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public partial class GenreTests
{
    [Fact]
    public async Task GetAllShouldReturnGenreDtosList()
    {
        // Arrange
        var genres = new List<Genre>();

        Genre parentGenre = new(Guid.NewGuid(), "ParentGenre", null, null);

        for (int i = 1; i <= 5; ++i)
        {
            genres.Add(new Genre(Guid.NewGuid(), $"Name-{i}", null, null, parentGenre));
        }

        _genreRepositoryMock.Setup(x => x.GetAllGenre()).ReturnsAsync(genres.AsEnumerable);
        _mongoGenreRepositoryMock.Setup(x => x.GetAllGenre()).ReturnsAsync([]);

        // Act
        var result = await _genreService.GetAll();
        var genreDtos = result.ToList();

        // Assert
        Assert.Equal(5, genreDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", genreDtos[i].Name);
            Assert.Equal(parentGenre.Id, genreDtos[i].ParentGenreId);
        }
    }

    [Fact]
    public async Task GetByParentGenreShouldReturnChildGenreDtosList()
    {
        // Arrange
        Genre parentGenre = new(Guid.NewGuid(), "ParentGenre", null, null);

        var genres = new List<Genre>();

        for (int i = 1; i <= 5; ++i)
        {
            genres.Add(new Genre(Guid.NewGuid(), $"Name-{i}", null, null, parentGenre));
        }

        _genresSearchCriteriaMock.Setup(x => x.GetByParentId(parentGenre.Id)).ReturnsAsync(genres.AsEnumerable);
        _mongoGenresSearchCriteriaMock.Setup(x => x.GetByParentId(parentGenre.Id)).ReturnsAsync([]);

        // Act
        var result = await _genreService.GetSubGenres(parentGenre.Id);
        var genreDtos = result.ToList();

        // Assert
        Assert.Equal(5, genreDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", genreDtos[i].Name);
            Assert.Equal(parentGenre.Id, genreDtos[i].ParentGenreId);
        }
    }

    [Fact]
    public async Task GetByGameKeyShouldReturnGenreDtosList()
    {
        // Arrange
        string gameKey = "TestGame";

        Genre parentGenre = new(Guid.NewGuid(), "ParentGenre", null, null);

        var genres = new List<Genre>();

        for (int i = 1; i <= 5; ++i)
        {
            genres.Add(new Genre(Guid.NewGuid(), $"Name-{i}", null, null, parentGenre));
        }

        _genresSearchCriteriaMock.Setup(x => x.GetByGameKey(gameKey)).ReturnsAsync(genres.AsEnumerable);
        _mongoGenresSearchCriteriaMock.Setup(x => x.GetByGameKey(gameKey)).ReturnsAsync([]);

        // Act
        var result = await _genreService.GetGamesGenres(gameKey);
        var genreDtos = result.ToList();

        // Assert
        Assert.Equal(5, genreDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", genreDtos[i].Name);
            Assert.Equal(parentGenre.Id, genreDtos[i].ParentGenreId);
        }
    }
}
