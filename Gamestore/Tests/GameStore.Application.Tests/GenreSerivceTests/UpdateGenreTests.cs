﻿using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public partial class GenreTests
{
    [Fact]
    public async Task UpdateGenreShouldUpdateGenreOnce()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();
        string updatedName = "UpdatedName";

        GenreDto genreDto = new()
        {
            Id = genreId,
            Name = updatedName,
        };

        _genreRepositoryMock.Setup(x => x.Get(genreId)).ReturnsAsync(new Genre(genreId, "TestName"));

        // Act
        await _genreService.UpdateGenre(genreDto);

        // Assert
        _genreRepositoryMock.Verify(x => x.Get(genreId), Times.Once);
        _genreRepositoryMock.Verify(x => x.Update(It.Is<Genre>(g => g.Id == genreId && g.Name == updatedName)), Times.Once);
    }

    [Fact]
    public async Task UpdateGenreCorrectParentGenreIdProvidedShouldUpdateGenreOnce()
    {
        // Arrange
        Guid parentGenreId = Guid.NewGuid();

        var parentGenre = new Genre(parentGenreId, "ParentGenre");

        string updatedName = "UpdatedName";

        GenreDto genreDto = new()
        {
            Id = Guid.NewGuid(),
            Name = updatedName,
            ParentGenreId = parentGenreId,
        };
        _genreRepositoryMock.Setup(x => x.Get((Guid)genreDto.Id)).ReturnsAsync(new Genre((Guid)genreDto.Id, "TestName", parentGenre));

        _genreRepositoryMock.Setup(x => x.Get(parentGenreId)).ReturnsAsync(parentGenre);

        // Act
        await _genreService.UpdateGenre(genreDto);

        // Assert
        _genreRepositoryMock.Verify(x => x.Get((Guid)genreDto.Id), Times.Once);
        _genreRepositoryMock.Verify(x => x.Get(parentGenreId), Times.Once);
        _genreRepositoryMock.Verify(x => x.Update(It.Is<Genre>(g => g.Id == genreDto.Id && g.Name == updatedName && g.ParentGenre == parentGenre)), Times.Once);
    }

    [Fact]
    public async Task UpdateGenreIncorrectParentGenreIdProvidedShouldThrowException()
    {
        // Arrange
        GenreDto genreDto = new()
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedName",
            ParentGenreId = Guid.NewGuid(),
        };

        _genreRepositoryMock.Setup(x => x.Get((Guid)genreDto.Id)).ReturnsAsync(new Genre((Guid)genreDto.Id, "TestName"));

        _genreRepositoryMock.Setup(x => x.Get((Guid)genreDto.ParentGenreId)).Returns(Task.FromResult<Genre>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _genreService.UpdateGenre(genreDto));
    }
}