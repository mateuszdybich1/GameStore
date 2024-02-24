using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public partial class GenreTests
{
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
        _genreRepositoryMock.Verify(x => x.GetGenre(genreId), Times.Once);
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
