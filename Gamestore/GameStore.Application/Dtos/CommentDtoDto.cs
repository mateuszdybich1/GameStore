using System.ComponentModel.DataAnnotations;
using GameStore.Domain.Entities;

namespace GameStore.Application.Dtos;

public class CommentDtoDto
{
    private CommentActionType _action;

    [Required]
    public CommentDto Comment { get; set; }

    public Guid? ParentId { get; set; }

    public CommentActionType? Action
    {
        get => _action;
        set => _action = value == null ? CommentActionType.Normal : (CommentActionType)value;
    }
}
