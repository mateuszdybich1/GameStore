using GameStore.Application.Dtos;
using GameStore.Application.IUserServices;
using GameStore.Domain;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IUserRepositories;
using GameStore.Domain.UserEntities;

namespace GameStore.Application.UserServices;

public class UserService(IRoleRepository roleRepository, IUserRepository userRepository, IAuthRepository authRepository) : IUserService
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAuthRepository _authRepository = authRepository;

    public async Task<UserModelDto> GetUser(Guid userId)
    {
        return new(await Get(userId));
    }

    public async Task<List<RoleModelDto>> GetUserRoles(Guid userId)
    {
        var userModel = await Get(userId);

        return userModel.Roles.Select(x => new RoleModelDto(x)).ToList();
    }

    public async Task<string> LoginUser(string username, string password)
    {
        var userModel = await _userRepository.GetUserWithRoles(username, password) ?? throw new EntityNotFoundException("Incorrect username or password");

        return (await _authRepository.GetJwtToken(userModel)).AccessToken;
    }

    public async Task<string> RegisterUser(UserRegisterDto userRegisterDto)
    {
        var roles = new List<RoleModel>();
        foreach (var role in userRegisterDto.Roles)
        {
            roles.Add(await _roleRepository.Get(role) ?? throw new EntityNotFoundException($"Couldn't find role by ID: {role}"));
        }

        PersonModel userModel = new(userRegisterDto.User.Name, userRegisterDto.Password, roles);

        try
        {
            await _userRepository.Add(userModel);
        }
        catch (Exception ex)
        {
            throw new EntityNotFoundException(ex.Message);
        }

        return (await _authRepository.GetJwtToken(userModel)).AccessToken;
    }

    public async Task<Guid> RemoveUser(Guid userId)
    {
        var userModel = await Get(userId);

        await _userRepository.Delete(userModel);

        return userModel.Id;
    }

    public async Task<Guid> UpdateUser(UserRegisterDto userRegisterDto)
    {
        var userModel = await Get(userRegisterDto.User.ID);

        userModel.Name = userRegisterDto.User.Name;
        userModel.Password = userRegisterDto.Password;

        var rolesList = new List<RoleModel>();

        foreach (var roleId in userRegisterDto.Roles)
        {
            rolesList.Add(await _roleRepository.Get(roleId) ?? throw new EntityNotFoundException($"Couldn't find role by ID: {roleId}"));
        }

        userModel.Roles = rolesList;

        await _userRepository.Update(userModel);

        return userModel.Id;
    }

    public async Task<bool> UserExists(string username)
    {
        return await _userRepository.UserExists(username);
    }

    public async Task<List<UserModelDto>> GetAllUsers()
    {
        return (await _userRepository.GetAllUsers()).Select(x => new UserModelDto(x)).ToList();
    }

    private async Task<PersonModel> Get(Guid? userId)
    {
        return userId == Guid.Empty
            ? throw new EntityNotFoundException($"Couldn't find user by ID: {userId}")
            : await _userRepository.Get((Guid)userId!) ?? throw new EntityNotFoundException($"Couldn't find user by ID: {userId}");
    }
}
