using GameStore.Application.Dtos;
using GameStore.Application.IUserServices;
using GameStore.Domain;
using GameStore.Domain.UserEntities;

namespace GameStore.Application.UserServices;

public class UserCheckService(IUserContext userContext) : IUserCheckService
{
    private readonly IUserContext _userContext = userContext;

    public bool CanUserAccess(AccessPageDto accessPageDto)
    {
        if (_userContext.IsAuthenticated)
        {
            Guid userId = _userContext.CurrentUserId;

            return userId != Guid.Empty && _userContext.Permissions.Contains(accessPageDto.TargetPage);
        }
        else
        {
            return CanAnonymousAccess(accessPageDto.TargetPage);
        }
    }

    public bool IsCurrentUser(Guid? userId)
    {
        return userId != null && _userContext.IsAuthenticated && _userContext.CurrentUserId == userId;
    }

    private static bool CanAnonymousAccess(Permissions targetPage)
    {
        return targetPage is Permissions.Games or Permissions.Game or
                Permissions.Genre or Permissions.Genres or
                Permissions.Platform or Permissions.Platforms or
                Permissions.Publishers or Permissions.Publisher or Permissions.Comments;
    }
}
