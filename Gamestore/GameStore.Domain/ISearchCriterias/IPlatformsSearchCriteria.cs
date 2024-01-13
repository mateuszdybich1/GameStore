using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;

public interface IPlatformsSearchCriteria
{
    public List<Platform> GetByGameKey(string gameKey);
}
