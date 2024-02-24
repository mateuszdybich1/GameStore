using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;

public interface IPlatformRepository : IRepository<Platform>
{
    public Task<IEnumerable<Platform>> GetAllPlatforms();
}
