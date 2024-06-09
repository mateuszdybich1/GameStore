using GameStore.Domain.UserEntities;

namespace GameStore.Domain;

public interface IAuthService
{
    Task<string> LoginUser(string username, string password);

    Task<TokenData> GetJwtToken(PersonModel userModel);
}
