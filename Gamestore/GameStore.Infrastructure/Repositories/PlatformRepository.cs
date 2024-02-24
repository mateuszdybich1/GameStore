using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class PlatformRepository(AppDbContext appDbContext) : Repository<Platform>(appDbContext), IPlatformRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IEnumerable<Platform>> GetAllPlatforms()
    {
        return await _appDbContext.Platforms.ToListAsync();
    }
}
