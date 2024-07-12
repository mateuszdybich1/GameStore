using GameStore.Application.Dtos;
using GameStore.Application.IUserServices;
using GameStore.Domain;
using GameStore.Domain.Exceptions;
using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Application.UserServices;

public class UserService(RoleManager<RoleModel> roleManager, UserManager<PersonModel> userManager, IAuthService authService) : IUserService
{
    private readonly IAuthService _authService = authService;
    private readonly RoleManager<RoleModel> _roleManager = roleManager;
    private readonly UserManager<PersonModel> _userManager = userManager;

    public async Task<UserModelDto> GetUser(Guid userId)
    {
        return new(await Get(userId));
    }

    public async Task<IEnumerable<RoleModelDto>> GetUserRoles(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var roles = new List<RoleModel>();
        if (user != null)
        {
            var roleNames = await _userManager.GetRolesAsync(user);

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roles.Add(role);
                }
            }
        }

        return roles.Select(x => new RoleModelDto(x)).ToList();
    }

    public async Task<string> LoginUser(string username, string password)
    {
        return await _authService.LoginUser(username, password);
    }

    public async Task<string> RegisterUser(UserRegisterDto userRegisterDto)
    {
        if (userRegisterDto == null || (userRegisterDto != null && (await _userManager.FindByNameAsync(userRegisterDto.User.Name)) != null))
        {
            throw new ExistingFieldException("User exists");
        }

        var roles = new List<RoleModel>();
        foreach (var role in userRegisterDto.Roles)
        {
            roles.Add(await _roleManager.FindByIdAsync(role.ToString()) ?? throw new EntityNotFoundException($"Couldn't find role by ID: {role}"));
        }

        PersonModel userModel = new(userRegisterDto.User.Name, userRegisterDto.Password);

        try
        {
            await _userManager.CreateAsync(userModel);

            await _userManager.AddToRolesAsync(userModel, roles.Select(x => x.Name.ToString()));
        }
        catch (Exception ex)
        {
            throw new EntityNotFoundException(ex.Message);
        }

        return (await _authService.GetJwtToken(userModel)).AccessToken;
    }

    public async Task<Guid> RemoveUser(Guid userId)
    {
        var userModel = await Get(userId);
        await _userManager.DeleteAsync(userModel);

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
        return await _userManager.FindByNameAsync(username) != null;
    }

    public IEnumerable<UserModelDto> GetAllUsers()
    {
        return _userManager.Users.ToList().Select(x => new UserModelDto(x)).ToList();
    }

    public async Task<Guid> BanUser(UserBanDto userBanDto)
    {
        var user = await _userManager.FindByNameAsync(userBanDto.User) ?? throw new EntityNotFoundException($"Couldn't find user: {userBanDto.User}");

        user.IsBanned = true;
        user.BanTime = DateTime.UtcNow;
        user.BanDuration = userBanDto.Duration;
        await _userManager.UpdateAsync(user);

        return user.Id;
    }

    public async Task<Guid> UnBanUser(Guid id)
    {
        var user = await Get(id);

        user.IsBanned = false;
        user.BanTime = null;
        user.BanDuration = string.Empty;

        await _userManager.UpdateAsync(user);

        return user.Id;
    }

    public async Task<IEnumerable<UserNotificationType>> GetUserNotifications(Guid userId)
    {
        var user = await Get(userId);

        return user.NotificationTypes.Select(Enum.Parse<UserNotificationType>);
    }

    public async Task<Guid> ChangeUserNotifications(UserNotificationDto userNotificationDto)
    {
        var user = await Get(userNotificationDto.UserId);

        user.NotificationTypes = userNotificationDto.Notifications.Select(x => x.ToString()).ToList();
        await _userManager.UpdateAsync(user);
        return user.Id;
    }

    private async Task<PersonModel> Get(Guid? userId)
    {
        return userId == Guid.Empty
            ? throw new EntityNotFoundException($"Couldn't find user by ID: {userId}")
            : await _userManager.FindByIdAsync(((Guid)userId!).ToString()) ?? throw new EntityNotFoundException($"Couldn't find user by ID: {userId}");
    }
}
