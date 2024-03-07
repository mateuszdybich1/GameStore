using GameStore.Domain.Entities;
using GameStore.Domain.ISearchCriterias;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.SearchCriteria;

public class GenresSearchCriteria(AppDbContext appDbContext) : IGenresSearchCriteria
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IEnumerable<Genre>> GetByGameKey(string gameKey)
    {
        return await _appDbContext.Genres.Include(x => x.ParentGenre).Include(x => x.Games).Where(x => x.Games.Any(y => y.Key == gameKey)).ToListAsync();
    }

    public async Task<Genre> GetByGenreName(string name)
    {
        return await _appDbContext.Genres.Where(x => x.Name == name).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Genre>> GetByParentId(Guid parentId)
    {
        return await _appDbContext.Genres.Include(x => x.ParentGenre).Where(x => x.ParentGenre.Id == parentId).ToListAsync();
    }
}
