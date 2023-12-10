using GameStore.Infrastructure.Entities;

namespace GameStore.Infrastructure.ISearchCriterias;
public interface IGamesSearchCriteria
{
    public Game GetByKey(string key);

    public List<Game> GetByGenreId(Guid genreId);

    public List<Game> GetByPlatformId(Guid platformId);
}
