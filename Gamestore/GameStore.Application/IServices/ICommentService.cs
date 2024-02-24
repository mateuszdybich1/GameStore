using GameStore.Application.Dtos;
using GameStore.Domain.Entities;

namespace GameStore.Application.IServices;

public interface ICommentService
{
    public Task<Guid> AddComment(string gameKey, CommentDtoDto commentDto);

    public Task<Guid> DeleteComment(string gameKey, Guid commentId);

    public Task<IEnumerable<CommentModel>> GetComments(string gameKey);
}
