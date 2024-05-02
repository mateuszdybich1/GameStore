using GameStore.Domain.UserEntities;

namespace GameStore.Domain;

public interface IAuthRepository
{
    Task<TokenData> GetJwtToken(PersonModel userModel);
}
