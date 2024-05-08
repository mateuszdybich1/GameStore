using GameStore.Domain.UserEntities;

namespace GameStore.Domain;
public interface IUserContext
{
    bool IsAuthenticated { get; }

    Guid CurrentUserId { get; }

    string? UserName { get; }

    bool IsBanned { get; }

    List<Permissions> Permissions { get; }
}
