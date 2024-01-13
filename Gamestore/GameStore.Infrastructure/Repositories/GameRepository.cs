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
