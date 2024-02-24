using GameStore.Domain.Entities;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PlatformServiceTests;
public partial class PlatformTests
{
    [Fact]
    public async Task GetAllShouldReturnPlatformDtosList()
    {
        // Arrange
        var platforms = new List<Platform>();

        for (int i = 1; i <= 5; ++i)
        {
            platforms.Add(new Platform(Guid.NewGuid(), $"Type-{i}"));
        }

        _platformRepositoryMock.Setup(x => x.GetAllPlatforms()).ReturnsAsync(platforms.AsEnumerable);

        // Act
        var result = await _platformService.GetAll();
        var platformDtos = result.ToList();

        // Assert
        Assert.Equal(5, platformDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Type-{i + 1}", platformDtos[i].Type);
        }
    }

    [Fact]
    public async Task GetGamesPlatformsCorrectGameKeyProvidedShouldReturnPlatformDtosList()
    {
        // Arrange
        string gamekey = "TestKey";

        var platforms = new List<Platform>();

        for (int i = 1; i <= 5; ++i)
        {
            platforms.Add(new Platform(Guid.NewGuid(), $"Type-{i}"));
        }

        _platformsSearchCriteriaMock.Setup(x => x.GetByGameKey(gamekey)).ReturnsAsync(platforms.AsEnumerable);

        // Act
        var result = await _platformService.GetGamesPlatforms(gamekey);
        var platformDtos = result.ToList();

        // Assert
        Assert.Equal(5, platformDtos.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"Type-{i + 1}", platformDtos[i].Type);
        }
    }
}
