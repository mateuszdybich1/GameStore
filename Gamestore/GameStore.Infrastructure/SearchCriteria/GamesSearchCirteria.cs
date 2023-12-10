using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.ISearchCriterias;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.SearchCriteria;
public class GamesSearchCirteria : IGamesSearchCriteria
{
    private readonly AppDbContext _appDbContext;

    public GamesSearchCirteria(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public List<Game> GetByGenre(string genre)
    {
        return _appDbContext.Games.Include(x => x.Genres.Where(y => y.Name == genre)).ToList();
    }

    public Game GetByKey(string key)
    {
        return _appDbContext.Games.SingleOrDefault(x => x.Key == key);
    }

    public List<Game> GetByPlatform(string platform)
    {
        return _appDbContext.Games.Include(x => x.Platforms.Where(y => y.Type == platform)).ToList();
    }
}
