using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IPlatformService
{
    public Task<Guid> AddPlatform(PlatformDto platformDto);

    public Task<Guid> UpdatePlatform(PlatformDto platformDto);

    public Task<Guid> DeletePlatform(Guid platformId);

    public Task<PlatformDto> GetPlatform(Guid platformId);

    public Task<IEnumerable<PlatformDto>> GetAll();

    public Task<IEnumerable<PlatformDto>> GetGamesPlatforms(string gameKey);
}
