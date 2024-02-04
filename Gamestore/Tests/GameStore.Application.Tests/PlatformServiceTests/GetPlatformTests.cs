using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public partial class PlatformTests
{
    [Fact]
    public void GetPlatformCorrectPlatformIdProvidedShouldReturnPlatofrmDto()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();
        string platformType = "TestType";

        Platform platform = new(platformId, platformType);

        _platformRepositoryMock.Setup(x => x.GetPlatform(platformId)).Returns(platform);

        // Act
        PlatformDto platformDto = _platformService.GetPlatform(platformId);

        // Assert
        Assert.NotNull(platformDto);
        _platformRepositoryMock.Verify(x => x.GetPlatform(platformId), Times.Once);
        Assert.True(platformDto.Id == platformId && platformDto.Type == platformType);
    }

    [Fact]
    public void GetPlatformIncorrectPlatformIdProvidedShouldThrowException()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();

        _platformRepositoryMock.Setup(x => x.GetPlatform(platformId)).Returns((Platform)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _platformService.GetPlatform(platformId));
    }
}
