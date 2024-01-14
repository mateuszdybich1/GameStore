using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;
public interface IPublisherSearchCriteria
{
    public Publisher GetPublisherByCompanyName(string companyName);

    public Publisher GetPublisherByGameKey(string gameKey);
}
