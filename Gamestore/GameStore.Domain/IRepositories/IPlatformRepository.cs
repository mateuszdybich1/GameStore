using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;

public interface IPlatformRepository : IRepository<Platform>
{
    Task<IEnumerable<Platform>> GetAllPlatforms();
}
