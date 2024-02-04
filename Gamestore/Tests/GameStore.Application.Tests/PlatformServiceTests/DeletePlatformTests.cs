using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public partial class PlatformTests
{
    [Fact]
    public void DeletePlatformShouldDeletePlatformOnce()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();

        Platform platform = new(platformId, "TestType");
        _platformRepositoryMock.Setup(x => x.GetPlatform(platformId)).Returns(platform);

        // Act
        _platformService.DeletePlatform(platformId);

        // Assert
        _platformRepositoryMock.Verify(x => x.GetPlatform(platformId), Times.Once);
        _platformRepositoryMock.Verify(x => x.RemovePlatform(It.Is<Platform>(p => p == platform)), Times.Once);
    }

    [Fact]
    public void DeletePlatformIncorrectPlatformIdProvidedShouldThrowException()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();
        _platformRepositoryMock.Setup(x => x.GetPlatform(platformId)).Returns((Platform)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _platformService.DeletePlatform(platformId));
    }
}
