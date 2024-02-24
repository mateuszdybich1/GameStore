using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public partial class GenreTests
{
    [Fact]
    public async Task GetGenreShouldReturnGenreDto()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();
        string genreName = "TestName";

        _genreRepositoryMock.Setup(x => x.Get(genreId)).ReturnsAsync(new Genre(genreId, genreName));

        // Act
        GenreDto genreDto = await _genreService.GetGenre(genreId);

        // Assert
        Assert.NotNull(genreDto);
        _genreRepositoryMock.Verify(x => x.Get(genreId), Times.Once);
        Assert.True(genreDto.Id == genreId && genreDto.Name == genreName);
    }

    [Fact]
    public async Task GetGenreIncorrectGenreIdProvidedShouldThrowException()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();

        _genreRepositoryMock.Setup(x => x.Get(genreId)).Returns(Task.FromResult<Genre>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _genreService.GetGenre(genreId));
    }
}
