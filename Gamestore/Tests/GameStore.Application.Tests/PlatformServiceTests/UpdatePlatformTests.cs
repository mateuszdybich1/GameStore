using GameStore.Application.Dtos;
using GameStore.Application.Exceptions;
using GameStore.Application.Services;
using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public class UpdatePlatformTests
{
    private readonly PlatformService _platformService;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IPlatformsSearchCriteria> _platformsSearchCriteriaMock;

    public UpdatePlatformTests()
    {
        _platformRepositoryMock = new();
        _platformsSearchCriteriaMock = new();

        _platformService = new(_platformRepositoryMock.Object, _platformsSearchCriteriaMock.Object);
    }

    [Fact]
    public void UpdatePlatformShouldUpdatePlatofrmOnce()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();
        string platformType = "TestType";

        Platform platform = new(platformId, platformType);

        _platformRepositoryMock.Setup(x => x.GetPlatform(platformId)).Returns(platform);

        string platformUpdatedType = "UpdatedType";

        PlatformDto platformDto = new()
        {
            Id = platformId,
            Type = platformUpdatedType,
        };

        // Act
        _platformService.UpdatePlatform(platformDto);

        // Assert
        _platformRepositoryMock.Verify(x => x.GetPlatform(platformId), Times.Once);
        _platformRepositoryMock.Verify(x => x.UpdatePlatform(It.Is<Platform>(p => p.Id == platformId && p.Type == platformUpdatedType)), Times.Once);
    }

    [Fact]
    public void UpdatePlatformIncorrectPlatformIdProvidedShouldThrowException()
    {
        // Arrange
        Guid platformId = Guid.NewGuid();

        PlatformDto platformDto = new()
        {
            Id = platformId,
            Type = "UpdatedPlatform",
        };

        _platformRepositoryMock.Setup(x => x.GetPlatform(platformId)).Returns((Platform)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _platformService.UpdatePlatform(platformDto));
    }
}
