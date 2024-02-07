using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;

namespace GameStore.Infrastructure.Repositories;

public class PublisherRepository(AppDbContext appDbContext) : IPublisherRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public void AddPublisher(Publisher publisher)
    {
        _appDbContext.Publishers.Add(publisher);
        _appDbContext.SaveChanges();
    }

    public void UpdatePublisher(Publisher publisher)
    {
        _appDbContext.Publishers.Update(publisher);
        _appDbContext.SaveChanges();
    }

    public void DeletePublisher(Publisher publisher)
    {
        _appDbContext.Publishers.Remove(publisher);
        _appDbContext.SaveChanges();
    }

    public List<Publisher> GetAllPublishers()
    {
        return [.. _appDbContext.Publishers];
    }

    public Publisher GetPublisher(Guid publisherId)
    {
        return _appDbContext.Publishers.SingleOrDefault(x => x.Id == publisherId);
    }
}
