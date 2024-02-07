using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public partial class GenreTests
{
    [Fact]
    public void GetAllShouldReturnGenreDtosList()
    {
        // Arrange
        var genres = new List<Genre>();

        Genre parentGenre = new(Guid.NewGuid(), "ParentGenre");

        for (int i = 1; i <= 5; ++i)
        {
            genres.Add(new Genre(Guid.NewGuid(), $"Name-{i}", parentGenre));
        }

        _genreRepositoryMock.Setup(x => x.GetAllGenre()).Returns(genres);

        // Act
        List<GenreDto> genreDtos = _genreService.GetAll();

        // Assert
        Assert.Equal(5, genreDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", genreDtos[i].Name);
            Assert.Equal(parentGenre.Id, genreDtos[i].ParentGenreId);
        }
    }

    [Fact]
    public void GetByParentGenreShouldReturnChildGenreDtosList()
    {
        // Arrange
        Genre parentGenre = new(Guid.NewGuid(), "ParentGenre");

        var genres = new List<Genre>();

        for (int i = 1; i <= 5; ++i)
        {
            genres.Add(new Genre(Guid.NewGuid(), $"Name-{i}", parentGenre));
        }

        _genresSearchCriteriaMock.Setup(x => x.GetByParentId(parentGenre.Id)).Returns(genres);

        // Act
        List<GenreDto> genreDtos = _genreService.GetSubGenres(parentGenre.Id);

        // Assert
        Assert.Equal(5, genreDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", genreDtos[i].Name);
            Assert.Equal(parentGenre.Id, genreDtos[i].ParentGenreId);
        }
    }

    [Fact]
    public void GetByGameKeyShouldReturnGenreDtosList()
    {
        // Arrange
        string gameKey = "TestGame";

        Genre parentGenre = new(Guid.NewGuid(), "ParentGenre");

        var genres = new List<Genre>();

        for (int i = 1; i <= 5; ++i)
        {
            genres.Add(new Genre(Guid.NewGuid(), $"Name-{i}", parentGenre));
        }

        _genresSearchCriteriaMock.Setup(x => x.GetByGameKey(gameKey)).Returns(genres);

        // Act
        List<GenreDto> genreDtos = _genreService.GetGamesGenres(gameKey);

        // Assert
        Assert.Equal(5, genreDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", genreDtos[i].Name);
            Assert.Equal(parentGenre.Id, genreDtos[i].ParentGenreId);
        }
    }
}
