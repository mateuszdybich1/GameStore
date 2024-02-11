using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class GameRepository(AppDbContext appDbContext) : IGameRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public void AddGame(Game game)
    {
        _appDbContext.Games.Add(game);
        _appDbContext.SaveChanges();
    }

    public List<Game> GetAllGames()
    {
        return [.. _appDbContext.Games];
    }

    public List<Game> GetAllGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, PublishDateFilteringMode? publishDate, GameSortingMode? sortMode, uint page, NumberOfGamesOnPageFilteringMode numberOfGamesOnPage, int minPrice, int maxPrice)
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
            int takeGamesNum = 0;
            switch (numberOfGamesOnPage)
            {
                case NumberOfGamesOnPageFilteringMode.Ten:
                    takeGamesNum = 10;
                    break;
                case NumberOfGamesOnPageFilteringMode.Twenty:
                    takeGamesNum = 20;
                    break;
                case NumberOfGamesOnPageFilteringMode.Fifty:
                    takeGamesNum = 50;
                    break;
                case NumberOfGamesOnPageFilteringMode.OneHundred:
                    takeGamesNum = 100;
                    break;
                case NumberOfGamesOnPageFilteringMode.All:
                    break;
                default:
                    break;
            }

            games = games.Skip(((int)page - 1) * takeGamesNum).Take(takeGamesNum);
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

        return [.. games];
    }

    public Game GetGame(Guid gameId)
    {
        return _appDbContext.Games.SingleOrDefault(x => x.Id == gameId);
    }

    public void RemoveGame(Game game)
    {
        _appDbContext.Games.Remove(game);
        _appDbContext.SaveChanges();
    }

    public void UpdateGame(Game game)
    {
        _appDbContext.Games.Update(game);
        _appDbContext.SaveChanges();
    }

    public Game GetGameWithRelations(Guid gameId)
    {
        return _appDbContext.Games.Where(x => x.Id == gameId).Include(x => x.Genres).Include(x => x.Platforms).Single();
    }
}
