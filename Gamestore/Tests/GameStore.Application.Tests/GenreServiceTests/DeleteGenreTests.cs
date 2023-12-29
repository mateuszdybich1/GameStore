using GameStore.Application.Exceptions;
using GameStore.Application.Services;
using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public class DeleteGenreTests
{
    private readonly GenreService _genreService;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IGenresSearchCriteria> _genresSearchCriteriaMock;

    public DeleteGenreTests()
    {
        _genreRepositoryMock = new();
        _genresSearchCriteriaMock = new();

        _genreService = new(_genreRepositoryMock.Object, _genresSearchCriteriaMock.Object);
    }

    [Fact]
    public void DeleteGenreShouldDeleteGenreOnce()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();
        string genreName = "TestGenre";

        Genre genre = new(genreId, genreName);
        _genreRepositoryMock.Setup(x => x.GetGenre(genreId)).Returns(genre);

        // Act
        _genreService.DeleteGenre(genreId);

        // Assert
        _genreRepositoryMock.Verify(x => x.RemoveGenre(It.Is<Genre>(g => g == genre)), Times.Once);
    }

    [Fact]
    public void DeleteGenreIncorrectGenreIdProvidedShouldThrowException()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();

        _genreRepositoryMock.Setup(x => x.GetGenre(genreId)).Returns((Genre)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _genreService.DeleteGenre(genreId));
    }
}
