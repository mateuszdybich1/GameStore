using System.ComponentModel.DataAnnotations;

namespace GameStore.Application.Dtos;

public class GameDtoDto
{
    [Required]
    public GameDto Game { get; set; }

    [Required]
    public List<Guid> Genres { get; set; }

    [Required]
    public List<Guid> Platforms { get; set; }

    [Required]
    public Guid? Publisher { get; set; }
}
