using GameStore.Domain.Entities;
using GameStore.Domain.ISearchCriterias;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.SearchCriteria;

public class PlatformsSearchCriteria(AppDbContext appDbContext) : IPlatformsSearchCriteria
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IEnumerable<Platform>> GetByGameKey(string gameKey)
    {
        return await _appDbContext.Platforms.Include(x => x.Games).Where(x => x.Games.Any(y => y.Key == gameKey)).ToListAsync();
    }
}
