using System.ComponentModel.DataAnnotations;
using GameStore.Infrastructure.Entities;

namespace GameStore.Application.Dtos;

public class GameDto
{
    public GameDto()
    {
    }

    public GameDto(Game game)
    {
        GameId = game.Id;
        Name = game.Name;
        Description = game.Description;
        Key = game.Key;
    }

    public Guid GameId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Key { get; set; }

    public string Description { get; set; }

    [Required]
    public List<Guid> GenresIds { get; set; }

    [Required]
    public List<Guid> PlatformsIds { get; set; }
}
