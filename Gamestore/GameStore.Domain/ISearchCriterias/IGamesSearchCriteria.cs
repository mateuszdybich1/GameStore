using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;

public interface IGamesSearchCriteria
{
    public Game GetByKey(string key);

    public Game GetByKeyWithRelations(string key);

    public List<Game> GetByGenreId(Guid genreId);

    public List<Game> GetByPlatformId(Guid platformId);

    public List<Game> GetByPublisherName(string companyName);
}
