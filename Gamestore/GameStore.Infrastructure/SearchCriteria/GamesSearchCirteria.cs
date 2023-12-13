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

    public List<Game> GetByGenreId(Guid genreId)
    {
        List<Game> games = _appDbContext.Games
        .Include(x => x.Genres)
        .Where(x => x.Genres.Any(y => y.Id == genreId))
        .ToList();

        return games;
    }

    public Game GetByKey(string key)
    {
        return _appDbContext.Games.SingleOrDefault(x => x.Key == key);
    }

    public Game GetByKeyWithRelations(string key)
    {
        return _appDbContext.Games.Where(x => x.Key == key).Include(x => x.Genres).Include(x => x.Platforms).Single();
    }

    public List<Game> GetByPlatformId(Guid platformId)
    {
        List<Game> games = _appDbContext.Games
        .Include(x => x.Platforms)
        .Where(x => x.Platforms.Any(y => y.Id == platformId))
        .ToList();

        return games;
    }
}
