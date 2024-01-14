using GameStore.Domain.Entities;
using GameStore.Domain.ISearchCriterias;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.SearchCriteria;

public class GamesSearchCirteria(AppDbContext appDbContext) : IGamesSearchCriteria
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public List<Game> GetByGenreId(Guid genreId)
    {
        List<Game> games =
        [
            .. _appDbContext.Games
                    .Include(x => x.Genres)
                    .Where(x => x.Genres.Any(y => y.Id == genreId)),
        ];

        return games;
    }

    public Game GetByKey(string key)
    {
        return _appDbContext.Games.SingleOrDefault(x => x.Key == key);
    }

    public Game GetByKeyWithRelations(string key)
    {
        return _appDbContext.Games.Where(x => x.Key == key).Include(x => x.Genres).Include(x => x.Platforms).Include(x => x.Publisher).SingleOrDefault();
    }

    public List<Game> GetByPlatformId(Guid platformId)
    {
        List<Game> games =
        [
            .. _appDbContext.Games
                    .Include(x => x.Platforms)
                    .Where(x => x.Platforms.Any(y => y.Id == platformId)),
        ];

        return games;
    }

    public List<Game> GetByPublisherName(string companyName)
    {
        List<Game> games =
        [
            .. _appDbContext.Games
                    .Include(x => x.Publisher)
                    .Where(x => x.Publisher.CompanyName == companyName),
        ];

        return games;
    }
}
