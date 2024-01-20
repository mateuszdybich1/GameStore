using GameStore.Infrastructure.Entities;

namespace GameStore.Infrastructure.ISearchCriterias;

public interface IGamesSearchCriteria
{
    public Game GetByKey(string key);

    public Game GetByKeyWithRelations(string key);

    public List<Game> GetByGenreId(Guid genreId);

    public List<Game> GetByPlatformId(Guid platformId);
}
