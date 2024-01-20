using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IPlatformService
{
    public Guid AddPlatform(PlatformDto platformDto);

    public Guid UpdatePlatform(PlatformDto platformDto);

    public Guid DeletePlatform(Guid platformId);

    public PlatformDto GetPlatform(Guid platformId);

    public List<PlatformDto> GetAll();

    public List<PlatformDto> GetGamesPlatforms(string gameKey);
}
