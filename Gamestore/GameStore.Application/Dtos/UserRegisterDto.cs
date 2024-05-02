namespace GameStore.Application.Dtos;

public class UserRegisterDto
{
    public UserRegisterDto()
    {
    }

    public UserRegisterDto(string name, string password, List<Guid> roles)
    {
        User = new UserModelDto(name);
        Password = password;
        Roles = roles;
    }

    public UserModelDto User { get; set; }

    public List<Guid> Roles { get; set; }

    public string Password { get; set; }
}
