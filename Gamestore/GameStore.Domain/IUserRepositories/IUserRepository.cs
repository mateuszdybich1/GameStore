using GameStore.Domain.IRepositories;
using GameStore.Domain.UserEntities;

namespace GameStore.Domain.IUserRepositories;
public interface IUserRepository : IRepository<PersonModel>
{
    Task<bool> UserExists(string username);

    Task<PersonModel> GetUserWithRoles(string username, string password);

    Task<List<PersonModel>> GetAllUsers();
}
