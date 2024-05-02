using GameStore.Domain.IUserRepositories;
using GameStore.Domain.UserEntities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Users.Repositories;
public class UserRepository(IdentityDbContext identityDbContext) : Repository<PersonModel>(identityDbContext), IUserRepository
{
    private readonly IdentityDbContext _identityDbContext = identityDbContext;

    public async Task<List<PersonModel>> GetAllUsers()
    {
        return await _identityDbContext.Users.ToListAsync();
    }

    public async Task<PersonModel> GetUserWithRoles(string username, string password)
    {
        return await _identityDbContext.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Name == username && x.Password == password);
    }

    public async Task<bool> UserExists(string username)
    {
        return await _identityDbContext.Users.AnyAsync(x => x.Name == username);
    }
}
