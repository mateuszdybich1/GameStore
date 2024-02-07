using GameStore.Domain.Entities;
using GameStore.Domain.ISearchCriterias;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.SearchCriteria;

public class GenresSearchCriteria(AppDbContext appDbContext) : IGenresSearchCriteria
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public List<Genre> GetByGameKey(string gameKey)
    {
        List<Genre> genres =
        [
            .. _appDbContext.Genres
                    .Include(x => x.ParentGenre)
                    .Include(x => x.Games)
                    .Where(x => x.Games.Any(y => y.Key == gameKey)),
        ];

        return genres;
    }

    public List<Genre> GetByParentId(Guid parentId)
    {
        return [.. _appDbContext.Genres.Include(x => x.ParentGenre).Where(x => x.ParentGenre.Id == parentId)];
    }
}
