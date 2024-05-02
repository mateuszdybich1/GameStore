using GameStore.Application.Dtos;

namespace GameStore.Application.IUserServices;

public interface IUserCheckService
{
    public bool CanUserAccess(AccessPageDto accessPageDto);

    public bool IsCurrentUser(Guid? userId);
}
