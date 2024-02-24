using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public partial class GenreTests
{
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
        _genreRepositoryMock.Verify(x => x.GetGenre(genreId), Times.Once);
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
