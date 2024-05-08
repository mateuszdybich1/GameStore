using GameStore.Domain.IRepositories;
using GameStore.Domain.UserEntities;

namespace GameStore.Domain.IUserRepositories;
public interface IUserRepository : IRepository<PersonModel>
{
    Task<bool> UserExists(string username);

    Task<PersonModel> GetUser(string username);

    Task<PersonModel> GetUserWithRoles(string username, string password);

    Task<PersonModel> GetUserWithRoles(Guid id);

    Task<IEnumerable<PersonModel>> GetAllUsers();
}
