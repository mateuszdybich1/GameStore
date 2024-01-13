using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public class DeletePlatformTests
{
    private readonly PlatformService _platformService;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IPlatformsSearchCriteria> _platformsSearchCriteriaMock;

    public DeletePlatformTests()
    {
        _platformRepositoryMock = new();
        _platformsSearchCriteriaMock = new();

        _platformService = new(_platformRepositoryMock.Object, _platformsSearchCriteriaMock.Object);
    }

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
