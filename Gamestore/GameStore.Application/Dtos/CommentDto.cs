using System.ComponentModel.DataAnnotations;

namespace GameStore.Application.Dtos;

public class CommentDto
{
    public string? Name { get; set; }

    [Required]
    public string Body { get; set; }
}
