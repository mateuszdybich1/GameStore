using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface IPublisherRepository : IRepository<Publisher>
{
    public Task<IEnumerable<Publisher>> GetAllPublishers();
}
