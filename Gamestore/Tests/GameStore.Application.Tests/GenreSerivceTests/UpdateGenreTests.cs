using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public partial class GenreTests
{
    [Fact]
    public async Task UpdateSqlGenreShouldUpdateGenreOnce()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();
        string updatedName = "UpdatedName";

        GenreDto genreDto = new()
        {
            Id = genreId,
            Name = updatedName,
        };

        _genresSearchCriteriaMock.Setup(x => x.GetWithParent(genreId)).ReturnsAsync(new Genre(genreId, "TestName", null, null));
        _mongoGenreRepositoryMock.Setup(x => x.Get(genreId)).Returns(Task.FromResult<Genre>(null));

        // Act
        await _genreService.UpdateGenre(genreDto);

        // Assert
        _genresSearchCriteriaMock.Verify(x => x.GetWithParent(genreId), Times.Once);
        _genreRepositoryMock.Verify(x => x.Update(It.Is<Genre>(g => g.Id == genreId && g.Name == updatedName)), Times.Once);
    }

    [Fact]
    public async Task UpdateSqlGenreCorrectParentGenreIdProvidedShouldUpdateGenreOnce()
    {
        // Arrange
        Guid parentGenreId = Guid.NewGuid();

        var parentGenre = new Genre(parentGenreId, "ParentGenre", null, null);

        string updatedName = "UpdatedName";

        GenreDto genreDto = new()
        {
            Id = Guid.NewGuid(),
            Name = updatedName,
            ParentGenreId = parentGenreId,
        };
        _genresSearchCriteriaMock.Setup(x => x.GetWithParent((Guid)genreDto.Id)).ReturnsAsync(new Genre((Guid)genreDto.Id, "TestName", null, null, parentGenre));
        _mongoGenreRepositoryMock.Setup(x => x.Get((Guid)genreDto.Id)).Returns(Task.FromResult<Genre>(null));
        _genreRepositoryMock.Setup(x => x.Get(parentGenreId)).ReturnsAsync(parentGenre);
        _mongoGenreRepositoryMock.Setup(x => x.Get(parentGenreId)).Returns(Task.FromResult<Genre>(null));

        // Act
        await _genreService.UpdateGenre(genreDto);

        // Assert
        _genresSearchCriteriaMock.Verify(x => x.GetWithParent((Guid)genreDto.Id), Times.Once);
        _genreRepositoryMock.Verify(x => x.Get(parentGenreId), Times.Once);
        _genreRepositoryMock.Verify(x => x.Update(It.Is<Genre>(g => g.Id == genreDto.Id && g.Name == updatedName && g.ParentGenre == parentGenre)), Times.Once);
    }

    [Fact]
    public async Task UpdateSqlGenreIncorrectParentGenreIdProvidedShouldThrowException()
    {
        // Arrange
        GenreDto genreDto = new()
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedName",
            ParentGenreId = Guid.NewGuid(),
        };

        _genresSearchCriteriaMock.Setup(x => x.GetWithParent((Guid)genreDto.Id)).ReturnsAsync(new Genre((Guid)genreDto.Id, "TestName", null, null));

        _mongoGenreRepositoryMock.Setup(x => x.Get((Guid)genreDto.Id)).Returns(Task.FromResult<Genre>(null));
        _genreRepositoryMock.Setup(x => x.Get((Guid)genreDto.ParentGenreId)).Returns(Task.FromResult<Genre>(null));
        _mongoGenreRepositoryMock.Setup(x => x.Get((Guid)genreDto.ParentGenreId)).Returns(Task.FromResult<Genre>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _genreService.UpdateGenre(genreDto));
    }
}