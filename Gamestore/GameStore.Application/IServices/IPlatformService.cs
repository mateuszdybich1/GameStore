using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IPlatformService
{
    Task<Guid> AddPlatform(PlatformDto platformDto);

    Task<Guid> UpdatePlatform(PlatformDto platformDto);

    Task<Guid> DeletePlatform(Guid platformId);

    Task<PlatformDto> GetPlatform(Guid platformId);

    Task<IEnumerable<PlatformDto>> GetAll();

    Task<IEnumerable<PlatformDto>> GetGamesPlatforms(string gameKey);
}
