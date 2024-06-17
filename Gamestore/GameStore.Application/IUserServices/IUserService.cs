using GameStore.Application.Dtos;

namespace GameStore.Application.IUserServices;

public interface IUserService
{
    Task<string> LoginUser(string username, string password);

    Task<string> RegisterUser(UserRegisterDto userRegisterDto);

    Task<Guid> UpdateUser(UserRegisterDto userRegisterDto);

    Task<UserModelDto> GetUser(Guid userId);

    IEnumerable<UserModelDto> GetAllUsers();

    Task<Guid> RemoveUser(Guid userId);

    Task<IEnumerable<RoleModelDto>> GetUserRoles(Guid userId);

    Task<bool> UserExists(string username);

    Task<Guid> BanUser(UserBanDto userBanDto);

    Task<Guid> UnBanUser(Guid id);
}
