using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public class GetGenresListTests
{
    private readonly GenreService _genreService;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IGenresSearchCriteria> _genresSearchCriteriaMock;

    public GetGenresListTests()
    {
        _genreRepositoryMock = new();
        _genresSearchCriteriaMock = new();

        _genreService = new(_genreRepositoryMock.Object, _genresSearchCriteriaMock.Object);
    }

    [Fact]
    public void GetAllShouldReturnGenreDtosList()
    {
        // Arrange
        var genres = new List<Genre>();

        Guid parentGenreId = Guid.NewGuid();

        for (int i = 1; i <= 5; ++i)
        {
            genres.Add(new Genre(Guid.NewGuid(), $"Name-{i}", parentGenreId));
        }

        _genreRepositoryMock.Setup(x => x.GetAllGenre()).Returns(genres);

        // Act
        List<GenreDto> genreDtos = _genreService.GetAll();

        // Assert
        Assert.Equal(5, genreDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", genreDtos[i].Name);
            Assert.Equal(parentGenreId, genreDtos[i].ParentGerneId);
        }
    }

    [Fact]
    public void GetByParentGenreShouldReturnChildGenreDtosList()
    {
        // Arrange
        Guid parentGenreId = Guid.NewGuid();

        var genres = new List<Genre>();

        for (int i = 1; i <= 5; ++i)
        {
            genres.Add(new Genre(Guid.NewGuid(), $"Name-{i}", parentGenreId));
        }

        _genresSearchCriteriaMock.Setup(x => x.GetByParentId(parentGenreId)).Returns(genres);

        // Act
        List<GenreDto> genreDtos = _genreService.GetSubGenres(parentGenreId);

        // Assert
        Assert.Equal(5, genreDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", genreDtos[i].Name);
            Assert.Equal(parentGenreId, genreDtos[i].ParentGerneId);
        }
    }

    [Fact]
    public void GetByGameKeyShouldReturnGenreDtosList()
    {
        // Arrange
        string gameKey = "TestGame";

        Guid parentGenreId = Guid.NewGuid();

        var genres = new List<Genre>();

        for (int i = 1; i <= 5; ++i)
        {
            genres.Add(new Genre(Guid.NewGuid(), $"Name-{i}", parentGenreId));
        }

        _genresSearchCriteriaMock.Setup(x => x.GetByGameKey(gameKey)).Returns(genres);

        // Act
        List<GenreDto> genreDtos = _genreService.GetGamesGenres(gameKey);

        // Assert
        Assert.Equal(5, genreDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Name-{i + 1}", genreDtos[i].Name);
            Assert.Equal(parentGenreId, genreDtos[i].ParentGerneId);
        }
    }
}
