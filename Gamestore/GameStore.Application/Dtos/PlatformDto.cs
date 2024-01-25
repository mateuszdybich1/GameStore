using System.ComponentModel.DataAnnotations;
using GameStore.Domain.Entities;

namespace GameStore.Application.Dtos;

public class PlatformDto
{
    public PlatformDto()
    {
    }

    public PlatformDto(Platform platform)
    {
        Id = platform.Id;
        Type = platform.Type;
    }

    public Guid? Id { get; set; }

    [Required]
    public string Type { get; set; }
}
