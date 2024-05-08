using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;
public interface IPublisherSearchCriteria
{
    Task<Publisher> GetPublisherByCompanyName(string companyName);

    Task<Publisher> GetPublisherByGameKey(string gameKey);
}
