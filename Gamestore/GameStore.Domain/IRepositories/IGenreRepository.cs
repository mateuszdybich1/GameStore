using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;

public interface IGenreRepository : IRepository<Genre>
{
    public Task<IEnumerable<Genre>> GetAllGenre();
}
