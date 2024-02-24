using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class GameRepository(AppDbContext appDbContext) : Repository<Game>(appDbContext), IGameRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IEnumerable<Game>> GetAllGames()
    {
        return await _appDbContext.Games.ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetAllGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, PublishDateFilteringMode? publishDate, GameSortingMode? sortMode, uint page, NumberOfGamesOnPageFilteringMode numberOfGamesOnPage, int minPrice, int maxPrice)
    {
        IQueryable<Game>? games = _appDbContext.Games.Where(x => x.Price >= minPrice && x.Price <= maxPrice);
        if (publisherIds != null)
        {
            games = games.Include(x => x.Publisher).Where(x => publisherIds.Contains(x.PublisherId));
        }

        if (genreIds != null)
        {
            games = games.Include(x => x.Genres).Where(x => x.Genres.Any(g => genreIds.Contains(g.Id)));
        }

        if (platformIds != null)
        {
            games = games.Include(x => x.Platforms).Where(x => x.Platforms.Any(p => platformIds.Contains(p.Id)));
        }

        if (name != null)
        {
            games = games.Where(x => x.Name.Contains(name));
        }

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

        if (numberOfGamesOnPage != NumberOfGamesOnPageFilteringMode.All)
        {
            games = games.Skip(((int)page - 1) * (int)numberOfGamesOnPage).Take((int)numberOfGamesOnPage);
        }

        if (sortMode != null)
        {
            switch (sortMode)
            {
                case GameSortingMode.MostPopular:
                    games = games.OrderByDescending(x => x.NumberOfViews);
                    break;
                case GameSortingMode.MostCommented:
                    games = games.Include(x => x.Comments).OrderByDescending(x => x.Comments.Count);
                    break;
                case GameSortingMode.PriceASC:
                    games = games.OrderBy(x => x.Price);
                    break;
                case GameSortingMode.PriceDESC:
                    games = games.OrderByDescending(x => x.Price);
                    break;
                case GameSortingMode.New:
                    games = games.OrderByDescending(x => x.CreationDate);
                    break;
                default:
                    break;
            }
        }

        return await games.ToListAsync();
    }

    public async Task<int> GetAllGamesCount()
    {
        return await _appDbContext.Games.CountAsync();
    }

    public async Task<Game> GetGameWithRelations(Guid gameId)
    {
        return await _appDbContext.Games.Where(x => x.Id == gameId).Include(x => x.Genres).Include(x => x.Platforms).SingleOrDefaultAsync();
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
