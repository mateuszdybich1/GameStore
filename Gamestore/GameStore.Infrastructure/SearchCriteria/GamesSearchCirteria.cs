using GameStore.Domain.Entities;
using GameStore.Domain.ISearchCriterias;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.SearchCriteria;

public class GamesSearchCirteria(AppDbContext appDbContext) : IGamesSearchCriteria
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IEnumerable<Game>> GetByGenreId(Guid genreId)
    {
        var games = await _appDbContext.Games.Include(x => x.Genres).Where(x => x.Genres.Any(y => y.Id == genreId)).ToListAsync();

        return games;
    }

    public async Task<Game> GetByKey(string key)
    {
        return await _appDbContext.Games.SingleOrDefaultAsync(x => x.Key == key);
    }

    public async Task<object> GetByKeyWithRelations(string key)
    {
        var game = await _appDbContext.Games.Where(x => x.Key == key).SingleOrDefaultAsync();

        if (game == null)
        {
            return null;
        }

        game = await _appDbContext.Games
        .Where(x => x.Key == key)
        .Include(x => x.Publisher)
        .Include(x => x.Platforms)
        .Include(x => x.Genres).ThenInclude(x => x.ParentGenre)
        .SingleOrDefaultAsync();

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

    public async Task<IEnumerable<Game>> GetByPlatformId(Guid platformId)
    {
        return await _appDbContext.Games.Include(x => x.Platforms).Where(x => x.Platforms.Any(y => y.Id == platformId)).ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetByPublisherName(string companyName)
    {
        return await _appDbContext.Games.Include(x => x.Publisher).Where(x => x.Publisher.CompanyName == companyName).ToListAsync();
    }

    private Genre GetGenre(Genre genre)
    {
        var currentGenre = genre;
        while (currentGenre?.ParentGenre != null)
        {
            currentGenre = currentGenre.ParentGenre;
        }

        currentGenre.ParentGenre = _appDbContext.Genres.Include(x => x.ParentGenre).SingleOrDefault(x => x.Id == currentGenre.Id).ParentGenre;

        return currentGenre.ParentGenre == null ? genre : GetGenre(genre);
    }
}
