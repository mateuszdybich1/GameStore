using GameStore.Application.Dtos;

namespace GameStore.Application.IUserServices;

public interface IUserService
{
    public Task<string> LoginUser(string username, string password);

    public Task<string> RegisterUser(UserRegisterDto userRegisterDto);

    public Task<Guid> UpdateUser(UserRegisterDto userRegisterDto);

    public Task<UserModelDto> GetUser(Guid userId);

    public Task<List<UserModelDto>> GetAllUsers();

    public Task<Guid> RemoveUser(Guid userId);

    public Task<List<RoleModelDto>> GetUserRoles(Guid userId);

    public Task<bool> UserExists(string username);
}
