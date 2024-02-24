using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;

public interface IGamesSearchCriteria
{
    public Task<Game> GetByKey(string key);

    public Task<object> GetByKeyWithRelations(string key);

    public Task<IEnumerable<Game>> GetByGenreId(Guid genreId);

    public Task<IEnumerable<Game>> GetByPlatformId(Guid platformId);

    public Task<IEnumerable<Game>> GetByPublisherName(string companyName);
}
