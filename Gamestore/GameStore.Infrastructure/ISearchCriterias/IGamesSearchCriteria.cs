using GameStore.Infrastructure.Entities;

namespace GameStore.Infrastructure.ISearchCriterias;
public interface IGamesSearchCriteria
{
    public Game GetByKey(string key);

    public List<Game> GetByGenre(string genre);

    public List<Game> GetByPlatform(string platform);
}
