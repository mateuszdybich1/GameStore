using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public partial class PlatformTests
{
    [Fact]
    public async Task DeletePlatformShouldDeletePlatformOnce()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();

        Platform platform = new(platformId, "TestType");
        _platformRepositoryMock.Setup(x => x.Get(platformId)).ReturnsAsync(platform);

        // Act
        await _platformService.DeletePlatform(platformId);

        // Assert
        _platformRepositoryMock.Verify(x => x.Get(platformId), Times.Once);
        _platformRepositoryMock.Verify(x => x.Delete(It.Is<Platform>(p => p == platform)), Times.Once);
    }

    [Fact]
    public async Task DeletePlatformIncorrectPlatformIdProvidedShouldThrowException()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();
        _platformRepositoryMock.Setup(x => x.Get(platformId)).Returns(Task.FromResult<Platform>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _platformService.DeletePlatform(platformId));
    }
}
