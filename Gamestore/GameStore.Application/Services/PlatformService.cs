﻿using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class PlatformService(IPlatformRepository platformRepository, IPlatformsSearchCriteria platformsSearchCriteria) : IPlatformService
{
    private readonly IPlatformRepository _platformRepository = platformRepository;
    private readonly IPlatformsSearchCriteria _platformsSearchCriteria = platformsSearchCriteria;

    public Guid AddPlatform(PlatformDto platformDto)
    {
        Guid platformId = (platformDto.Id == null || platformDto.Id == Guid.Empty) ? Guid.NewGuid() : (Guid)platformDto.Id;

        Platform platform = new(platformId, platformDto.Type);

        try
        {
            _platformRepository.AddPlatform(platform);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique platform type");
        }

        return platform.Id;
    }

    public Guid DeletePlatform(Guid platformId)
    {
        Platform platform = _platformRepository.GetPlatform(platformId) ?? throw new EntityNotFoundException($"Couldn't find platform by ID: {platformId}");

        _platformRepository.RemovePlatform(platform);

        return platform.Id;
    }

    public List<PlatformDto> GetAll()
    {
        return _platformRepository.GetAllPlatforms().Select(x => new PlatformDto(x)).ToList();
    }

    public List<PlatformDto> GetGamesPlatforms(string gameKey)
    {
        List<Platform> platforms = _platformsSearchCriteria.GetByGameKey(gameKey);

        return platforms.Select(x => new PlatformDto(x)).ToList();
    }

    public PlatformDto GetPlatform(Guid platformId)
    {
        Platform platform = _platformRepository.GetPlatform(platformId) ?? throw new EntityNotFoundException($"Couldn't find platform by ID: {platformId}");
        return new(platform);
    }

    public Guid UpdatePlatform(PlatformDto platformDto)
    {
        if (platformDto.Id == null)
        {
            throw new ArgumentNullException("Cannot update platform. Id is null");
        }

        Platform platform = _platformRepository.GetPlatform((Guid)platformDto.Id) ?? throw new EntityNotFoundException($"Couldn't find platform by ID: {platformDto.Id}");

        platform.Type = platformDto.Type;
        platform.ModificationDate = DateTime.Now;

        try
        {
            _platformRepository.UpdatePlatform(platform);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique platform type");
        }

        return platform.Id;
    }
}
