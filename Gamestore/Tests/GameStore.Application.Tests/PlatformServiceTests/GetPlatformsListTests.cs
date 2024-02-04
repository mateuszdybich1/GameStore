using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public partial class PlatformTests
{
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
