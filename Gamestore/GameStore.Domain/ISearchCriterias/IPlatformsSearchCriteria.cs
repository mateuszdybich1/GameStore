using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;

public interface IPlatformsSearchCriteria
{
    Task<IEnumerable<Platform>> GetByGameKey(string gameKey);
}
