using GameStore.Application.Dtos;

namespace GameStore.Application.IUserServices;

public interface IUserCheckService
{
    bool CanUserAccess(AccessPageDto accessPageDto);

    bool IsCurrentUser(Guid? userId);

    Guid GetCurrentUserId();
}
