namespace GameStore.Application.Dtos;

public class AuthUserModel
{
    public string Login { get; set; }

    public string Password { get; set; }

    public bool InternalAuth { get; set; }
}
