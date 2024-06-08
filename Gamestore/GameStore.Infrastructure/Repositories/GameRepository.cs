using System.Diagnostics;
using GameStore.Domain.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class GameRepository(AppDbContext appDbContext) : Repository<Game>(appDbContext), IGameRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override async Task Add(Game entity)
    {
        var existingPublisher = await _appDbContext.Publishers.FirstOrDefaultAsync(x => x.Id == entity.PublisherId);

        if (existingPublisher == null)
        {
            _appDbContext.Publishers.Add(entity.Publisher);
        }

        if (entity.Genres != null)
        {
            foreach (var genre in entity.Genres)
            {
                var tempGenre = await _appDbContext.Genres.FirstOrDefaultAsync(x => x.Id == genre.Id);

                if (tempGenre == null)
                {
                    _appDbContext.Genres.Add(genre);
                }
            }
        }

        _appDbContext.Games.Add(entity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Game>> GetAllGames()
    {
        return await _appDbContext.Games.ToListAsync();
    }

    public async Task<GameModelsDto> GetAllGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, PublishDateFilteringMode? publishDate, GameSortingMode? sortMode, uint page, NumberOfGamesOnPageFilteringMode numberOfGamesOnPage, int minPrice, int maxPrice)
    {
        var watch = Stopwatch.StartNew();
        IQueryable<Game>? games = _appDbContext.Games.Where(x => x.Price >= minPrice && x.Price <= maxPrice);

        if (publishDate != null)
        {
            DateTime from = DateTime.Now;

            switch (publishDate)
            {
                case PublishDateFilteringMode.LastWeek:
                    from = from.AddDays(-7);
                    break;
                case PublishDateFilteringMode.LastMonth:
                    from = from.AddMonths(-1);
                    break;
                case PublishDateFilteringMode.LastYear:
                    from = from.AddYears(-1);
                    break;
                case PublishDateFilteringMode.TwoYears:
                    from = from.AddYears(-2);
                    break;
                case PublishDateFilteringMode.ThreeYears:
                    from = from.AddYears(-3);
                    break;
                default:
                    break;
            }

            games = games.Where(x => x.CreationDate >= from);
        }

        if (name != null)
        {
            games = games.Where(x => x.Name.Contains(name));
        }

        if (publisherIds != null && publisherIds.Count > 0)
        {
            games = games.Include(x => x.Publisher).Where(x => publisherIds.Any(y => y == x.PublisherId));
        }

        if (genreIds != null && genreIds.Count > 0)
        {
            games = games.Include(x => x.Genres).Where(game => game.Genres.Any(genre => genreIds.Contains(genre.Id)));
        }

        if (platformIds != null && platformIds.Count > 0)
        {
            games = games.Include(x => x.Platforms).Where(x => x.Platforms.Any(p => platformIds.Contains(p.Id)));
        }

        var totalPages = 1;
        if (numberOfGamesOnPage != NumberOfGamesOnPageFilteringMode.All)
        {
            var gamesCount = games.Count();
            totalPages = gamesCount / (int)numberOfGamesOnPage;
            if (gamesCount % (int)numberOfGamesOnPage > 0)
            {
                totalPages += 1;
            }
        }

        var downloadedGames = await games.Include(x => x.Comments).ToListAsync();

        watch.Stop();

        Debug.WriteLine($"Get Games: {watch.ElapsedMilliseconds} ms");
        return new GameModelsDto(downloadedGames, totalPages, (int)page);
    }

    public async Task<int> GetAllGamesCount()
    {
        return await _appDbContext.Games.CountAsync();
    }

    public async Task<Game> GetGameWithRelations(Guid gameId)
    {
        return await _appDbContext.Games.Where(x => x.Id == gameId).Include(x => x.Genres).Include(x => x.Platforms).SingleOrDefaultAsync();
    }

    public async Task<Game> GetGameWithRelations(string gameId)
    {
        return await _appDbContext.Games.Where(x => x.Key == gameId).Include(x => x.Genres).Include(x => x.Platforms).SingleOrDefaultAsync();
    }

    public async Task<int> GetNumberOfPages(NumberOfGamesOnPageFilteringMode numberOfGamesOnPage)
    {
        int numberOfGames = await _appDbContext.Games.CountAsync();
        int numberOfPages = numberOfGames / (int)numberOfGamesOnPage;
        if (numberOfGames % (int)numberOfGamesOnPage > 0)
        {
            numberOfPages += 1;
        }

        return numberOfPages;
    }
}
