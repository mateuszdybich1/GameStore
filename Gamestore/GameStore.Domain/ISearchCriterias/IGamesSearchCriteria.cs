using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;

public interface IGamesSearchCriteria
{
    Task<Game> GetByKey(string key);

    Task<object> GetByKeyWithRelations(string key);

    Task<IEnumerable<Game>> GetByGenreId(Guid genreId);

    Task<IEnumerable<Game>> GetByPlatformId(Guid platformId);

    Task<IEnumerable<Game>> GetByPublisherName(string companyName);
}
