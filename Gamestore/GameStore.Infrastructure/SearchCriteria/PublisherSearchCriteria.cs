using GameStore.Domain.Entities;
using GameStore.Domain.ISearchCriterias;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.SearchCriteria;

public class PublisherSearchCriteria(AppDbContext appDbContext) : IPublisherSearchCriteria
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public Publisher GetPublisherByCompanyName(string companyName)
    {
        return _appDbContext.Publishers.Where(x => x.CompanyName == companyName).FirstOrDefault();
    }

    public Publisher GetPublisherByGameKey(string gameKey)
    {
        return _appDbContext.Publishers.Include(x => x.Games).SingleOrDefault(x => x.Games.Any(y => y.Key == gameKey));
    }
}
