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

    public object GetByKeyWithRelations(string key)
    {
        var game = _appDbContext.Games
        .Where(x => x.Key == key).SingleOrDefault();

        if (game == null)
        {
            return null;
        }

        game = _appDbContext.Games
        .Where(x => x.Key == key)
        .Include(x => x.Publisher)
        .Include(x => x.Platforms)
        .Include(x => x.Genres).ThenInclude(x => x.ParentGenre)
        .SingleOrDefault();

        return new
        {
            Game = new
            {
                Id = game.Id,
                Name = game.Name,
                Key = game.Key,
                Description = game.Description,
                Price = game.Price,
                UnitInStock = game.UnitInStock,
                Discount = game.Discount,
            },
            Genres = game.Genres.Select(genre => new
            {
                Id = genre.Id,
                Name = genre.Name,
                ParentGenre = GetGenre(genre).ParentGenre,
            }),
            Platforms = game.Platforms.Select(platform => new
            {
                Id = platform.Id,
                Type = platform.Type,
            }),
            Publisher = new
            {
                Id = game.Publisher.Id,
                Name = game.Publisher.CompanyName,
                HomePage = game.Publisher.HomePage,
                Description = game.Publisher.Description,
            },
        };
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

    private IEnumerable<Genre> GetChild(Guid id)
    {
        return _appDbContext.Genres.Where(x => x.ParentGenre.Id == id || x.Id == id)
                    .Union(_appDbContext.Genres.Where(x => x.ParentGenre.Id == id)
                                .SelectMany(y => GetChild(y.Id)));
    }

    private Genre GetGenre(Genre genre)
    {
        var currentGenre = genre;
        while (currentGenre?.ParentGenre != null)
        {
            currentGenre = currentGenre.ParentGenre;
        }

        currentGenre.ParentGenre = _appDbContext.Genres.Include(x => x.ParentGenre).FirstOrDefault(x => x.Id == currentGenre.Id).ParentGenre;

        return currentGenre.ParentGenre == null ? genre : GetGenre(genre);
    }
}
