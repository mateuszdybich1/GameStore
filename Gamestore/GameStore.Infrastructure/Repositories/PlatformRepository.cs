using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;

namespace GameStore.Infrastructure.Repositories;
public class PlatformRepository : IPlatformRepository
{
    private readonly AppDbContext _appDbContext;

    public PlatformRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddPlatform(Platform platform)
    {
        _appDbContext.Platforms.Add(platform);
        _appDbContext.SaveChanges();
    }

    public List<Platform> GetAllPlatforms()
    {
        return _appDbContext.Platforms.ToList();
    }

    public Platform GetPlatform(Guid platformId)
    {
        return _appDbContext.Platforms.SingleOrDefault(x => x.Id == platformId);
    }

    public void RemovePlatform(Platform platform)
    {
        _appDbContext.Platforms.Remove(platform);
        _appDbContext.SaveChanges();
    }

    public void UpdatePlatform(Platform platform)
    {
        _appDbContext.Platforms.Update(platform);
        _appDbContext.SaveChanges();
    }
}
