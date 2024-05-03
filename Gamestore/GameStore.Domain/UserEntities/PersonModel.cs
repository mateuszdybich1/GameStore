namespace GameStore.Domain.UserEntities;
public class PersonModel : Microsoft.AspNetCore.Identity.IdentityUser<Guid>
{
    public PersonModel()
    {
    }

    public PersonModel(string name, string password, List<RoleModel> roles)
    {
        Id = Guid.NewGuid();
        Name = name;
        Password = password;
        Roles = roles;
    }

    public PersonModel(string name, string password)
    {
        Id = Guid.NewGuid();
        Name = name;
        Password = password;
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
        Roles = [new RoleModel(DefaultRoles.User.ToString())];
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly
    }

    public string Name { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Password { get; set; }

    public bool IsBanned { get; set; }

    public DateTime? BanTime { get; set; }

    public string BanDuration { get; set; }

    public List<RoleModel> Roles { get; set; }
}
