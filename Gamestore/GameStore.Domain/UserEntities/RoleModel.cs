namespace GameStore.Domain.UserEntities;
public class RoleModel : Microsoft.AspNetCore.Identity.IdentityRole<Guid>
{
    public RoleModel()
    {
    }

    public RoleModel(string name)
    {
        Name = name;
    }

    public override string? Name
    {
        get => base.Name;

        set
        {
            if (value != null)
            {
                base.Name = value;
            }
        }
    }

    public List<PersonModel> Users { get; set; }
}
