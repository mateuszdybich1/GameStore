using GameStore.Domain.IUserRepositories;
using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Users.Repositories;
public class UserRepository(UserManager<PersonModel> userManager) : IUserRepository
{
    private readonly UserManager<PersonModel> _userManager = userManager;

    public async Task<IEnumerable<PersonModel>> GetAllUsers()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<PersonModel> GetUser(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<PersonModel> GetUserWithRoles(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return null;
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
        return !isValidPassword ? null : user;
    }

    public async Task<PersonModel> GetUserWithRoles(Guid id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    public async Task<bool> UserExists(string username)
    {
        return await _userManager.FindByNameAsync(username) != null;
    }

    public async Task Add(PersonModel entity)
    {
        await _userManager.CreateAsync(entity);
    }

    public async Task Delete(PersonModel entity)
    {
        await _userManager.DeleteAsync(entity);
    }

    public async Task Update(PersonModel entity)
    {
        await _userManager.UpdateAsync(entity);
    }

    public async Task<PersonModel> Get(Guid id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }
}
