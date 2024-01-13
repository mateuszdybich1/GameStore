using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public class GetPlatformsListTests
{
    private readonly PlatformService _platformService;
    private readonly Mock<IPlatformRepository> _platformRepositoryMock;
    private readonly Mock<IPlatformsSearchCriteria> _platformsSearchCriteriaMock;

    public GetPlatformsListTests()
    {
        _platformRepositoryMock = new();
        _platformsSearchCriteriaMock = new();

        _platformService = new(_platformRepositoryMock.Object, _platformsSearchCriteriaMock.Object);
    }

    [Fact]
    public void GetAllShouldReturnPlatformDtosList()
    {
        // Arrange
        var platforms = new List<Platform>();

        for (int i = 1; i <= 5; ++i)
        {
            platforms.Add(new Platform(Guid.NewGuid(), $"Type-{i}"));
        }

        _platformRepositoryMock.Setup(x => x.GetAllPlatforms()).Returns(platforms);

        // Act
        List<PlatformDto> platformDtos = _platformService.GetAll();

        // Assert
        Assert.Equal(5, platformDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Type-{i + 1}", platformDtos[i].Type);
        }
    }

    [Fact]
    public void GetGamesPlatformsCorrectGameKeyProvidedShouldReturnPlatformDtosList()
    {
        // Arrange
        string gamekey = "TestKey";

        var platforms = new List<Platform>();

        for (int i = 1; i <= 5; ++i)
        {
            platforms.Add(new Platform(Guid.NewGuid(), $"Type-{i}"));
        }

        _platformsSearchCriteriaMock.Setup(x => x.GetByGameKey(gamekey)).Returns(platforms);

        // Act
        List<PlatformDto> platformDtos = _platformService.GetGamesPlatforms(gamekey);

        // Assert
        Assert.Equal(5, platformDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Type-{i + 1}", platformDtos[i].Type);
        }
    }
}
