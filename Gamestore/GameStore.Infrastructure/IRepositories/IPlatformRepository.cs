using GameStore.Infrastructure.Entities;

namespace GameStore.Infrastructure.IRepositories;

public interface IPlatformRepository
{
    public void AddPlatform(Platform platform);

    public Platform GetPlatform(Guid platformId);

    public void UpdatePlatform(Platform platform);

    public void RemovePlatform(Platform platform);

    public List<Platform> GetAllPlatforms();
}
