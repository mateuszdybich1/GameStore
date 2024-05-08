using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;

public interface IGenresSearchCriteria
{
    Task<IEnumerable<Genre>> GetByGameKey(string gameKey);

    Task<IEnumerable<Genre>> GetByParentId(Guid parentId);

    Task<Genre> GetByGenreName(string name);

    Task<Genre> GetWithParent(Guid genreId);
}
