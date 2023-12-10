using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;

namespace GameStore.Infrastructure.Repositories;
public class GameRepository : IGameRepository
{
    private readonly AppDbContext _appDbContext;

    public GameRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddGame(Game game)
    {
        _appDbContext.Games.Add(game);
        _appDbContext.SaveChanges();
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
}
