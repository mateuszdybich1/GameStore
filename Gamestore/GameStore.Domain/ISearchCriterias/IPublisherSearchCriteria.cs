using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;
public interface IPublisherSearchCriteria
{
    public Task<Publisher> GetPublisherByCompanyName(string companyName);

    public Task<Publisher> GetPublisherByGameKey(string gameKey);
}
