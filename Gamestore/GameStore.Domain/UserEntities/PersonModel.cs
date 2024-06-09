using Microsoft.AspNetCore.Identity;

namespace GameStore.Domain.UserEntities;
public class PersonModel : IdentityUser<Guid>
{
    public PersonModel()
    {
    }

    public PersonModel(string name, string password)
    {
        Id = Guid.NewGuid();
        UserName = name;
        SetPassword(password);
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool IsBanned { get; set; }

    public DateTime? BanTime { get; set; }

    public string BanDuration { get; set; }

    public void SetPassword(string password)
    {
        var passwordHasher = new PasswordHasher<PersonModel>();
        PasswordHash = passwordHasher.HashPassword(this, password);
    }
}
