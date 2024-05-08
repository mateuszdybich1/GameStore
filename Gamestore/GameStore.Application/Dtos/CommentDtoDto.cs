using System.ComponentModel.DataAnnotations;
using GameStore.Domain.Entities;

namespace GameStore.Application.Dtos;

public class CommentDtoDto
{
    [Required]
    public CommentDto Comment { get; set; }

    public Guid? ParentId { get; set; }

    public CommentActionType? Action { get; set; } = CommentActionType.Normal;
}
