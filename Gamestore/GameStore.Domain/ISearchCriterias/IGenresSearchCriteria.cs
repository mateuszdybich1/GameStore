using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;

public interface IGenresSearchCriteria
{
    public List<Genre> GetByGameKey(string gameKey);

    public List<Genre> GetByParentId(Guid parentId);
}
