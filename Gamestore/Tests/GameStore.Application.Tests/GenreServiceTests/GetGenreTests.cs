using GameStore.Application.Dtos;
using GameStore.Application.Exceptions;
using GameStore.Application.Services;
using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public class GetGenreTests
{
    private readonly GenreService _genreService;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IGenresSearchCriteria> _genresSearchCriteriaMock;

    public GetGenreTests()
    {
        _genreRepositoryMock = new();
        _genresSearchCriteriaMock = new();

        _genreService = new(_genreRepositoryMock.Object, _genresSearchCriteriaMock.Object);
    }

    [Fact]
    public void GetGenreShouldReturnGenreDto()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();
        string genreName = "TestName";

        _genreRepositoryMock.Setup(x => x.GetGenre(genreId)).Returns(new Genre(genreId, genreName));

        // Act
        GenreDto genreDto = _genreService.GetGenre(genreId);

        // Assert
        Assert.NotNull(genreDto);
        Assert.True(genreDto.Id == genreId && genreDto.Name == genreName);
    }

    [Fact]
    public void GetGenreIncorrectGenreIdProvidedShouldThrowException()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();

        _genreRepositoryMock.Setup(x => x.GetGenre(genreId)).Returns((Genre)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _genreService.GetGenre(genreId));
    }
}
