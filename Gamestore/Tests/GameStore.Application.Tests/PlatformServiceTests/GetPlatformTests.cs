using GameStore.Application.Dtos;
using GameStore.Application.Exceptions;
using GameStore.Application.Services;
using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public class GetPlatformTests
{
    private readonly PlatformService _platformService;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IPlatformsSearchCriteria> _platformsSearchCriteriaMock;

    public GetPlatformTests()
    {
        _platformRepositoryMock = new();
        _platformsSearchCriteriaMock = new();

        _platformService = new(_platformRepositoryMock.Object, _platformsSearchCriteriaMock.Object);
    }

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
