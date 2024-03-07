using GameStore.Domain.Entities;
using GameStore.Domain.ISearchCriterias;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace GameStore.Infrastructure.SearchCriteria;

public class PublisherSearchCriteria(AppDbContext appDbContext) : IPublisherSearchCriteria
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Publisher> GetPublisherByCompanyName(string companyName)
    {
        return await _appDbContext.Publishers.Where(x => x.CompanyName == companyName).SingleOrDefaultAsync();
    }

    public async Task<Publisher> GetPublisherByGameKey(string gameKey)
    {
        return await _appDbContext.Publishers.Include(x => x.Games).SingleOrDefaultAsync(x => x.Games.Any(y => y.Key == gameKey));
    }
}
