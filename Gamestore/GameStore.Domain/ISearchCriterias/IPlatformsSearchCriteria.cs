using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;

public interface IPlatformsSearchCriteria
{
    public Task<IEnumerable<Platform>> GetByGameKey(string gameKey);
}
