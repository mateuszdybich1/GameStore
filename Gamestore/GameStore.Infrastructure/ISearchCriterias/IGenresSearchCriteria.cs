using GameStore.Infrastructure.Entities;

namespace GameStore.Infrastructure.ISearchCriterias;

public interface IGenresSearchCriteria
{
    public List<Genre> GetByGameKey(string gameKey);

    public List<Genre> GetByParentId(Guid parentId);
}
