using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GenreServiceTests;
public partial class GenreTests
{
    private readonly GenreService _genreService;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IGenreRepository> _mongoGenreRepositoryMock;
    private readonly Mock<IGenresSearchCriteria> _genresSearchCriteriaMock;
    private readonly Mock<IGenresSearchCriteria> _mongoGenresSearchCriteriaMock;
    private readonly Mock<IChangeLogService> _changeLogServiceMock;

    public GenreTests()
    {
        _genreRepositoryMock = new();
        _mongoGenreRepositoryMock = new();
        Mock<Func<RepositoryTypes, IGenreRepository>> mockGenreRepositoryFactory = new MockRepositoryFactory<IGenreRepository>().GetGamesRepository(_genreRepositoryMock, _mongoGenreRepositoryMock);

        _genresSearchCriteriaMock = new();
        _mongoGenresSearchCriteriaMock = new();
        Mock<Func<RepositoryTypes, IGenresSearchCriteria>> mockGenreSearchCriteriaRepositoryFactory = new MockRepositoryFactory<IGenresSearchCriteria>().GetGamesRepository(_genresSearchCriteriaMock, _mongoGenresSearchCriteriaMock);

        _changeLogServiceMock = new();

        _genreService = new(mockGenreRepositoryFactory.Object, mockGenreSearchCriteriaRepositoryFactory.Object, _changeLogServiceMock.Object);
    }

    [Fact]
    public async Task AddGenreShouldAddGenreOnce()
    {
        // Arrange
        string genreName = "TestName";

        GenreDto genreDto = new()
        {
            Name = genreName,
        };

        // Act
        var genreId = await _genreService.AddGenre(genreDto);

        // Assert
        Assert.True(genreId != Guid.Empty);
        _genreRepositoryMock.Verify(x => x.Add(It.Is<Genre>(x => x.Id == genreId && x.Name == genreDto.Name)), Times.Once());
    }

    [Fact]
    public async Task AddGenreParentGenreIdProvidedShouldAddGenreOnce()
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

        Genre parentGenre = new(parentGenreId, parentName, null, null);
        _genreRepositoryMock.Setup(x => x.Get(parentGenreId)).ReturnsAsync(parentGenre);

        // Act
        await _genreService.AddGenre(genreDto);

        // Assert
        _genreRepositoryMock.Verify(x => x.Get(parentGenreId), Times.Once);
        _genreRepositoryMock.Verify(x => x.Add(It.Is<Genre>(g => g.Name == genreName && g.ParentGenre == parentGenre)), Times.Once);
    }

    [Fact]
    public async Task AddGenreIncorectParentGenreIdProvidedShouldThrowException()
    {
        Guid parentGenreId = Guid.NewGuid();

        GenreDto genreDto = new()
        {
            Name = "TestGenre",
            ParentGenreId = parentGenreId,
        };

        _genreRepositoryMock.Setup(x => x.Get(parentGenreId)).Returns(Task.FromResult<Genre>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _genreService.AddGenre(genreDto));
    }
}