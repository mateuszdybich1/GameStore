﻿using GameStore.Application.Dtos;
using GameStore.Application.Exceptions;
using GameStore.Application.Services;
using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;
public class GetGameTests
{
    private readonly GameService _gameService;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IGamesSearchCriteria> _gamesSearchCriteriaMock;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;

    public GetGameTests()
    {
        _gameRepositoryMock = new();
        _gamesSearchCriteriaMock = new();
        _platformRepositoryMock = new();
        _genreRepositoryMock = new();

        _gameService = new(_gameRepositoryMock.Object, _gamesSearchCriteriaMock.Object, _platformRepositoryMock.Object, _genreRepositoryMock.Object);
    }

    [Fact]
    public void GetByGameKeyShouldReturnGameDto()
    {
        // Arrange
        string gameKey = "TestKey";

        var platforms = new List<Platform>();
        var genres = new List<Genre>();

        var game = new Game(Guid.NewGuid(), "TestName", gameKey, genres, platforms);
        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns(game);

        // Act
        GameDto gameDto = _gameService.GetGameByKey(gameKey);

        // Assert
        Assert.NotNull(gameDto);
        Assert.Equal(gameKey, gameDto.Key);
    }

    [Fact]
    public void GetByGameKeyIncorrectKeyProvidedShouldThrowException()
    {
        // Arrange
        string gameKey = "TestKey";
        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns((Game)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.GetGameByKey(gameKey));
    }

    [Fact]
    public void GetByGameIdShouldReturnGameDto()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();

        var platforms = new List<Platform>();
        var genres = new List<Genre>();

        var game = new Game(gameId, "TestName", "TestKey", genres, platforms);
        _gameRepositoryMock.Setup(x => x.GetGame(gameId)).Returns(game);

        // Act
        GameDto gameDto = _gameService.GetGameById(gameId);

        // Assert
        Assert.NotNull(gameDto);
        Assert.Equal(gameId, gameDto.GameId);
    }

    [Fact]
    public void GetByGameIdIncorrectKeyProvidedShouldThrowException()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        _gameRepositoryMock.Setup(x => x.GetGame(gameId)).Returns((Game)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.GetGameById(gameId));
    }

    [Fact]
    public void GetGameByKeyWithRelationsShouldReturnCorrectObject()
    {
        // Arrange
        string gameKey = "TestKey";

        var genres = new List<Genre> { new(Guid.NewGuid(), "Genre1"), new(Guid.NewGuid(), "Genre2") };
        var platforms = new List<Platform> { new(Guid.NewGuid(), "type1"), new(Guid.NewGuid(), "type2") };
        var game = new Game(Guid.NewGuid(), "Test Name", gameKey, genres, platforms);

        _gamesSearchCriteriaMock.Setup(x => x.GetByKeyWithRelations(gameKey)).Returns(game);

        // Act
        var result = _gameService.GetGameByKeyWithRelations(gameKey);

        // Assert
        Assert.NotNull(result);
    }
}
