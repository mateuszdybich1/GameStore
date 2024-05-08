using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface IPublisherRepository : IRepository<Publisher>
{
    Task<IEnumerable<Publisher>> GetAllPublishers();
}
