using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public partial class PlatformTests
{
    [Fact]
    public async Task GetPlatformCorrectPlatformIdProvidedShouldReturnPlatofrmDto()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();
        string platformType = "TestType";

        Platform platform = new(platformId, platformType);

        _platformRepositoryMock.Setup(x => x.Get(platformId)).ReturnsAsync(platform);

        // Act
        PlatformDto platformDto = await _platformService.GetPlatform(platformId);

        // Assert
        Assert.NotNull(platformDto);
        _platformRepositoryMock.Verify(x => x.Get(platformId), Times.Once);
        Assert.True(platformDto.Id == platformId && platformDto.Type == platformType);
    }

    [Fact]
    public async Task GetPlatformIncorrectPlatformIdProvidedShouldThrowException()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();

        _platformRepositoryMock.Setup(x => x.Get(platformId)).Returns(Task.FromResult<Platform>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _platformService.GetPlatform(platformId));
    }
}
