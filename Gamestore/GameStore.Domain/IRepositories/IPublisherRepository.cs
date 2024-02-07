using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface IPublisherRepository
{
    public void AddPublisher(Publisher publisher);

    public void UpdatePublisher(Publisher publisher);

    public void DeletePublisher(Publisher publisher);

    public Publisher GetPublisher(Guid publisherId);

    public List<Publisher> GetAllPublishers();
}
