using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public class AddGenreTests
{
    private readonly GenreService _genreService;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IGenresSearchCriteria> _genresSearchCriteriaMock;

    public AddGenreTests()
    {
        _genreRepositoryMock = new();
        _genresSearchCriteriaMock = new();

        _genreService = new(_genreRepositoryMock.Object, _genresSearchCriteriaMock.Object);
    }

    [Fact]
    public void AddGenreShouldAddGenreOnce()
    {
        // Arrange
        string genreName = "TestName";

        GenreDto genreDto = new()
        {
            Name = genreName,
        };

        // Act
        var genreId = _genreService.AddGenre(genreDto);

        // Assert
        Assert.True(genreId != Guid.Empty);
        _genreRepositoryMock.Verify(x => x.AddGenre(It.Is<Genre>(x => x.Id == genreId && x.Name == genreDto.Name)), Times.Once());
    }

    [Fact]
    public void AddGenreParentGenreIdProvidedShouldAddGenreOnce()
    {
        // Arrange
        Guid parentGenreId = Guid.NewGuid();
        string parentName = "ParentName";

        string genreName = "TestName";

        GenreDto genreDto = new()
        {
            Name = genreName,
            ParentGenreId = parentGenreId,
        };

        Genre parentGenre = new(parentGenreId, parentName);
        _genreRepositoryMock.Setup(x => x.GetGenre(parentGenreId)).Returns(parentGenre);

        // Act
        _genreService.AddGenre(genreDto);

        // Assert
        _genreRepositoryMock.Verify(x => x.GetGenre(parentGenreId), Times.Once);
        _genreRepositoryMock.Verify(x => x.AddGenre(It.Is<Genre>(g => g.Name == genreName && g.ParentGenre == parentGenre)), Times.Once);
    }

    [Fact]
    public void AddGenreIncorectParentGenreIdProvidedShouldThrowException()
    {
        Guid parentGenreId = Guid.NewGuid();

        GenreDto genreDto = new()
        {
            Name = "TestGenre",
            ParentGenreId = parentGenreId,
        };

        _genreRepositoryMock.Setup(x => x.GetGenre(parentGenreId)).Returns((Genre)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _genreService.AddGenre(genreDto));
    }
}