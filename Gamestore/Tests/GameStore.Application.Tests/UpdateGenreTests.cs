﻿using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public partial class GenreTests
{
    [Fact]
    public void UpdateGenreShouldUpdateGenreOnce()
    {
        // Arrange
        Guid genreId = Guid.NewGuid();
        string updatedName = "UpdatedName";

        GenreDto genreDto = new()
        {
            Id = genreId,
            Name = updatedName,
        };

        _genreRepositoryMock.Setup(x => x.GetGenre(genreId)).Returns(new Genre(genreId, "TestName"));

        // Act
        _genreService.UpdateGenre(genreDto);

        // Assert
        _genreRepositoryMock.Verify(x => x.GetGenre(genreId), Times.Once);
        _genreRepositoryMock.Verify(x => x.UpdateGenre(It.Is<Genre>(g => g.Id == genreId && g.Name == updatedName)), Times.Once);
    }

    [Fact]
    public void UpdateGenreCorrectParentGenreIdProvidedShouldUpdateGenreOnce()
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
        _genreRepositoryMock.Setup(x => x.GetGenre((Guid)genreDto.Id)).Returns(new Genre((Guid)genreDto.Id, "TestName", parentGenre));

        _genreRepositoryMock.Setup(x => x.GetGenre(parentGenreId)).Returns(parentGenre);

        // Act
        _genreService.UpdateGenre(genreDto);

        // Assert
        _genreRepositoryMock.Verify(x => x.GetGenre((Guid)genreDto.Id), Times.Once);
        _genreRepositoryMock.Verify(x => x.GetGenre(parentGenreId), Times.Once);
        _genreRepositoryMock.Verify(x => x.UpdateGenre(It.Is<Genre>(g => g.Id == genreDto.Id && g.Name == updatedName && g.ParentGenre == parentGenre)), Times.Once);
    }

    [Fact]
    public void UpdateGenreIncorrectParentGenreIdProvidedShouldThrowException()
    {
        // Arrange
        GenreDto genreDto = new()
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedName",
            ParentGenreId = Guid.NewGuid(),
        };

        _genreRepositoryMock.Setup(x => x.GetGenre((Guid)genreDto.Id)).Returns(new Genre((Guid)genreDto.Id, "TestName"));

        _genreRepositoryMock.Setup(x => x.GetGenre((Guid)genreDto.ParentGenreId)).Returns((Genre)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _genreService.UpdateGenre(genreDto));
    }
}