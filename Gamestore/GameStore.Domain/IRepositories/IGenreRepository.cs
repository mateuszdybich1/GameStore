using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;

public interface IGenreRepository : IRepository<Genre>
{
    Task<IEnumerable<Genre>> GetAllGenre();
}
