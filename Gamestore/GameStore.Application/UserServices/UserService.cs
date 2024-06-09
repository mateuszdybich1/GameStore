using GameStore.Application.Dtos;
using GameStore.Application.IUserServices;
using GameStore.Domain;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IUserRepositories;
using GameStore.Domain.UserEntities;

namespace GameStore.Application.UserServices;

public class UserService(IRoleRepository roleRepository, IUserRepository userRepository, IAuthService authService) : IUserService
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAuthService _authService = authService;

    public async Task<UserModelDto> GetUser(Guid userId)
    {
        return new(await Get(userId));
    }

    public async Task<IEnumerable<RoleModelDto>> GetUserRoles(Guid userId)
    {
        var userModel = await _roleRepository.GetUserRoles(userId) ?? throw new EntityNotFoundException($"Couldn't find user by ID: {userId}");

        return userModel.Select(x => new RoleModelDto(x)).ToList();
    }

    public async Task<string> LoginUser(string username, string password)
    {
        return await _authService.LoginUser(username, password);
    }

    public async Task<string> RegisterUser(UserRegisterDto userRegisterDto)
    {
        if (userRegisterDto == null || (userRegisterDto != null && (await _userRepository.GetUser(userRegisterDto.User.Name)) != null))
        {
            throw new ExistingFieldException("User exists");
        }

        var roles = new List<RoleModel>();
        foreach (var role in userRegisterDto.Roles)
        {
            roles.Add(await _roleRepository.Get(role) ?? throw new EntityNotFoundException($"Couldn't find role by ID: {role}"));
        }

        PersonModel userModel = new(userRegisterDto.User.Name, userRegisterDto.Password);

        try
        {
            await _userRepository.Add(userModel);

            await _roleRepository.AddUserRoles(userModel, roles);
        }
        catch (Exception ex)
        {
            throw new EntityNotFoundException(ex.Message);
        }

        return (await _authService.GetJwtToken(userModel)).AccessToken;
    }

    public async Task<Guid> RemoveUser(Guid userId)
    {
        var userModel = await Get(userId) ?? throw new EntityNotFoundException("User not found");
        await _userRepository.Delete(userModel);

        return userModel.Id;
    }

    public async Task<Guid> UpdateUser(UserRegisterDto userRegisterDto)
    {
        if (userRegisterDto.User.ID == null)
        {
            throw new Exception("User ID is null");
        }

        await RemoveUser((Guid)userRegisterDto.User.ID);
        await RegisterUser(userRegisterDto);
        return (Guid)userRegisterDto.User.ID!;
    }

    public async Task<bool> UserExists(string username)
    {
        return await _userRepository.UserExists(username);
    }

    public async Task<IEnumerable<UserModelDto>> GetAllUsers()
    {
        return (await _userRepository.GetAllUsers()).Select(x => new UserModelDto(x)).ToList();
    }

    public async Task<Guid> BanUser(UserBanDto userBanDto)
    {
        var user = await _userRepository.GetUser(userBanDto.User) ?? throw new EntityNotFoundException($"Couldn't find user: {userBanDto.User}");

        user.IsBanned = true;
        user.BanTime = DateTime.UtcNow;
        user.BanDuration = userBanDto.Duration;
        await _userRepository.Update(user);

        return user.Id;
    }

    public async Task<Guid> UnBanUser(Guid id)
    {
        var user = await Get(id);

        user.IsBanned = false;
        user.BanTime = null;
        user.BanDuration = string.Empty;

        await _userRepository.Update(user);

        return user.Id;
    }

    private async Task<PersonModel> Get(Guid? userId)
    {
        return userId == Guid.Empty
            ? throw new EntityNotFoundException($"Couldn't find user by ID: {userId}")
            : await _userRepository.Get((Guid)userId!) ?? throw new EntityNotFoundException($"Couldn't find user by ID: {userId}");
    }
}
