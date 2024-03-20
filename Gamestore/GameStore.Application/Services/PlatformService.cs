using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class PlatformService(IPlatformRepository platformRepository, IPlatformsSearchCriteria platformsSearchCriteria, IChangeLogService changeLogService) : IPlatformService
{
    private readonly IPlatformRepository _platformRepository = platformRepository;
    private readonly IPlatformsSearchCriteria _platformsSearchCriteria = platformsSearchCriteria;
    private readonly IChangeLogService _changeLogService = changeLogService;

    public async Task<Guid> AddPlatform(PlatformDto platformDto)
    {
        Guid platformId = (platformDto.Id == null || platformDto.Id == Guid.Empty) ? Guid.NewGuid() : (Guid)platformDto.Id;

        Platform platform = new(platformId, platformDto.Type);

        try
        {
            await _platformRepository.Add(platform);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique platform type");
        }

        return platform.Id;
    }

    public async Task<Guid> DeletePlatform(Guid platformId)
    {
        Platform platform = await _platformRepository.Get(platformId) ?? throw new EntityNotFoundException($"Couldn't find platform by ID: {platformId}");

        await _platformRepository.Delete(platform);

        return platform.Id;
    }

    public async Task<IEnumerable<PlatformDto>> GetAll()
    {
        var platforms = await _platformRepository.GetAllPlatforms();
        return platforms.Select(x => new PlatformDto(x));
    }

    public async Task<IEnumerable<PlatformDto>> GetGamesPlatforms(string gameKey)
    {
        var platforms = await _platformsSearchCriteria.GetByGameKey(gameKey);

        return platforms.Select(x => new PlatformDto(x));
    }

    public async Task<PlatformDto> GetPlatform(Guid platformId)
    {
        Platform platform = await _platformRepository.Get(platformId) ?? throw new EntityNotFoundException($"Couldn't find platform by ID: {platformId}");
        return new(platform);
    }

    public async Task<Guid> UpdatePlatform(PlatformDto platformDto)
    {
        if (platformDto.Id == null)
        {
            throw new ArgumentNullException("Cannot update platform. Id is null");
        }

        Platform platform = await _platformRepository.Get((Guid)platformDto.Id) ?? throw new EntityNotFoundException($"Couldn't find platform by ID: {platformDto.Id}");
        Platform oldPlatform = new(platform);
        platform.Type = platformDto.Type;
        platform.ModificationDate = DateTime.Now;

        try
        {
            await _platformRepository.Update(platform);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique platform type");
        }

        await _changeLogService.LogEntityChanges(LogActionType.Update, EntityType.Platform, oldPlatform, platform);

        return platform.Id;
    }
}
