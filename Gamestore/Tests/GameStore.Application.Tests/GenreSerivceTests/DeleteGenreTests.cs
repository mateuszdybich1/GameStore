﻿using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public partial class GenreTests
{
    [Fact]
    public async Task DeleteGenreShouldDeleteGenreOnce()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();
        string genreName = "TestGenre";

        Genre genre = new(genreId, genreName, null, null);
        _genreRepositoryMock.Setup(x => x.Get(genreId)).ReturnsAsync(genre);
        _mongoGenreRepositoryMock.Setup(x => x.Get(genreId)).Returns(Task.FromResult<Genre>(null));

        // Act
        await _genreService.DeleteGenre(genreId);

        // Assert
        _genreRepositoryMock.Verify(x => x.Get(genreId), Times.Once);
        _mongoGenreRepositoryMock.Verify(x => x.Get(genreId), Times.Once);
        _genreRepositoryMock.Verify(x => x.Delete(It.Is<Genre>(g => g == genre)), Times.Once);
    }

    [Fact]
    public async Task DeleteMongoGenreShouldDeleteGenreOnce()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();
        string genreName = "TestGenre";

        Genre genre = new(genreId, genreName, null, null);
        _mongoGenreRepositoryMock.Setup(x => x.Get(genreId)).ReturnsAsync(genre);
        _genreRepositoryMock.Setup(x => x.Get(genreId)).Returns(Task.FromResult<Genre>(null));

        // Act
        await _genreService.DeleteGenre(genreId);

        // Assert
        _genreRepositoryMock.Verify(x => x.Get(genreId), Times.Once);
        _mongoGenreRepositoryMock.Verify(x => x.Get(genreId), Times.Once);
        _mongoGenreRepositoryMock.Verify(x => x.Delete(It.Is<Genre>(g => g == genre)), Times.Once);
    }

    [Fact]
    public async Task DeleteGenreAndMongoGenreShouldDeleteGenreOnce()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();
        string genreName = "TestGenre";

        Genre genre = new(genreId, genreName, null, null);
        _mongoGenreRepositoryMock.Setup(x => x.Get(genreId)).ReturnsAsync(genre);
        _genreRepositoryMock.Setup(x => x.Get(genreId)).ReturnsAsync(genre);

        // Act
        await _genreService.DeleteGenre(genreId);

        // Assert
        _genreRepositoryMock.Verify(x => x.Get(genreId), Times.Once);
        _mongoGenreRepositoryMock.Verify(x => x.Get(genreId), Times.Once);
        _mongoGenreRepositoryMock.Verify(x => x.Delete(It.Is<Genre>(g => g == genre)), Times.Once);
        _genreRepositoryMock.Verify(x => x.Delete(It.Is<Genre>(g => g == genre)), Times.Once);
    }

    [Fact]
    public async Task DeleteGenreIncorrectGenreIdProvidedShouldThrowException()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();

        _genreRepositoryMock.Setup(x => x.Get(genreId)).Returns(Task.FromResult<Genre>(null));
        _mongoGenreRepositoryMock.Setup(x => x.Get(genreId)).Returns(Task.FromResult<Genre>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _genreService.DeleteGenre(genreId));
    }
}
