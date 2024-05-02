namespace GameStore.Domain.UserEntities;
public class PermissionModel
{
    public PermissionModel()
    {
    }

    public PermissionModel(Permissions permission)
    {
        Id = Guid.NewGuid();
        PermissionName = permission;
    }

    public Guid Id { get; private set; }

    public Permissions PermissionName { get; private set; }
}
