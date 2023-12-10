using GameStore.Infrastructure.Entities;

namespace GameStore.Infrastructure.ISearchCriterias;
public interface IPlatformsSearchCriteria
{
    public List<Platform> GetByGameKey(string gameKey);
}
