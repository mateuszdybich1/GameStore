using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.ISearchCriterias;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.SearchCriteria;

public class PlatformsSearchCriteria(AppDbContext appDbContext) : IPlatformsSearchCriteria
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public List<Platform> GetByGameKey(string gameKey)
    {
        List<Platform> platforms =
        [
            .. _appDbContext.Platforms
                    .Include(x => x.Games)
                    .Where(x => x.Games.Any(y => y.Key == gameKey)),
        ];

        return platforms;
    }
}
