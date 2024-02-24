using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public partial class PlatformTests
{
    [Fact]
    public async Task UpdatePlatformShouldUpdatePlatofrmOnce()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();
        string platformType = "TestType";

        Platform platform = new(platformId, platformType);

        _platformRepositoryMock.Setup(x => x.Get(platformId)).ReturnsAsync(platform);

        string platformUpdatedType = "UpdatedType";

        PlatformDto platformDto = new()
        {
            Id = platformId,
            Type = platformUpdatedType,
        };

        // Act
        await _platformService.UpdatePlatform(platformDto);

        // Assert
        _platformRepositoryMock.Verify(x => x.Get(platformId), Times.Once);
        _platformRepositoryMock.Verify(x => x.Update(It.Is<Platform>(p => p.Id == platformId && p.Type == platformUpdatedType)), Times.Once);
    }

    [Fact]
    public async Task UpdatePlatformIncorrectPlatformIdProvidedShouldThrowException()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();

        PlatformDto platformDto = new()
        {
            Id = platformId,
            Type = "UpdatedPlatform",
        };

        _platformRepositoryMock.Setup(x => x.Get(platformId)).Returns(Task.FromResult<Platform>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _platformService.UpdatePlatform(platformDto));
    }
}
