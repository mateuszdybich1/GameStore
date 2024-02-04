﻿using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.GameServiceTests;
public partial class GameTests
{
    [Fact]
    public void DeleteGameShouldDeleteGameOnce()
    {
        // Arrange
        string gameKey = "GameKey";

        Game game = new(Guid.NewGuid(), "Name", gameKey, 5, 5, 5, Guid.NewGuid(), new([new()]), new([new()]));
        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns(game);

        // Act
        _gameService.DeleteGame(gameKey);

        // Assert
        _gamesSearchCriteriaMock.Verify(x => x.GetByKey(gameKey), Times.Once);
        _gameRepositoryMock.Verify(x => x.RemoveGame(It.Is<Game>(g => g == game)), Times.Once);
    }

    [Fact]
    public void DeleteGameIncorrectGameKeyProvidedShouldTHrowException()
    {
        // Arrange
        string gameKey = "GameKey";

        _gamesSearchCriteriaMock.Setup(x => x.GetByKey(gameKey)).Returns((Game)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _gameService.DeleteGame(gameKey));
    }
}
