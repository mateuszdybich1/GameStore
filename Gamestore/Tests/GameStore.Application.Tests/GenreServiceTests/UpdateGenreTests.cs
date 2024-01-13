using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public class UpdateGenreTests
{
    private readonly GenreService _genreService;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IGenresSearchCriteria> _genresSearchCriteriaMock;

    public UpdateGenreTests()
    {
        _genreRepositoryMock = new();
        _genresSearchCriteriaMock = new();

        _genreService = new(_genreRepositoryMock.Object, _genresSearchCriteriaMock.Object);
    }

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

        string updatedName = "UpdatedName";

        GenreDto genreDto = new()
        {
            Id = Guid.NewGuid(),
            Name = updatedName,
            ParentGerneId = parentGenreId,
        };
        _genreRepositoryMock.Setup(x => x.GetGenre(genreDto.Id)).Returns(new Genre(genreDto.Id, "TestName", Guid.NewGuid()));

        _genreRepositoryMock.Setup(x => x.GetGenre(parentGenreId)).Returns(new Genre(parentGenreId, "ParentGenre"));

        // Act
        _genreService.UpdateGenre(genreDto);

        // Assert
        _genreRepositoryMock.Verify(x => x.GetGenre(genreDto.Id), Times.Once);
        _genreRepositoryMock.Verify(x => x.GetGenre(parentGenreId), Times.Once);
        _genreRepositoryMock.Verify(x => x.UpdateGenre(It.Is<Genre>(g => g.Id == genreDto.Id && g.Name == updatedName && g.ParentGerneId == parentGenreId)), Times.Once);
    }

    [Fact]
    public void UpdateGenreIncorrectParentGenreIdProvidedShouldThrowException()
    {
        // Arrange
        GenreDto genreDto = new()
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedName",
            ParentGerneId = Guid.NewGuid(),
        };

        _genreRepositoryMock.Setup(x => x.GetGenre(genreDto.Id)).Returns(new Genre(genreDto.Id, "TestName"));

        _genreRepositoryMock.Setup(x => x.GetGenre(genreDto.ParentGerneId)).Returns((Genre)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _genreService.UpdateGenre(genreDto));
    }
}