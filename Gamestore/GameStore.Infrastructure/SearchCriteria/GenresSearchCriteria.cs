using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.ISearchCriterias;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.SearchCriteria;
public class GenresSearchCriteria : IGenresSearchCriteria
{
    private readonly AppDbContext _appDbContext;

    public GenresSearchCriteria(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public List<Genre> GetByGameKey(string gameKey)
    {
        List<Genre> genres = _appDbContext.Genres
        .Include(x => x.Games)
        .Where(x => x.Games.Any(y => y.Key == gameKey))
        .ToList();

        return genres;
    }

    public List<Genre> GetByParentId(Guid parentId)
    {
        return _appDbContext.Genres.Where(x => x.ParentGerneId == parentId).ToList();
    }
}
