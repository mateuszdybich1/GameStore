using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public partial class PlatformTests
{
    private readonly PlatformService _platformService;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IPlatformsSearchCriteria> _platformsSearchCriteriaMock;
    private readonly Mock<IChangeLogService> _changeLogService;

    public PlatformTests()
    {
        _platformRepositoryMock = new();
        _platformsSearchCriteriaMock = new();
        _changeLogService = new();

        _platformService = new(_platformRepositoryMock.Object, _platformsSearchCriteriaMock.Object, _changeLogService.Object);
    }

    [Fact]
    public async Task AddPlatformShouldAddPlatformOnce()
    {
        // Arrange
        PlatformDto platformDto = new()
        {
            Type = "TestType",
        };

        // Act
        Guid platformId = await _platformService.AddPlatform(platformDto);

        // Assert
        Assert.True(platformId != Guid.Empty);
        _platformRepositoryMock.Verify(x => x.Add(It.Is<Platform>(x => x.Type == platformDto.Type)), Times.Once());
    }
}
