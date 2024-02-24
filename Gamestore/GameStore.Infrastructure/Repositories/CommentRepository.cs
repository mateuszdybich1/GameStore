using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class CommentRepository(AppDbContext appDbContext) : ICommentRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task AddComment(Comment comment)
    {
        _appDbContext.Comments.Add(comment);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<Comment> GetComment(Guid commentId, Guid gameId)
    {
        return await _appDbContext.Comments.FirstOrDefaultAsync(x => x.Id == commentId && x.Game.Id == gameId);
    }

    public async Task<IEnumerable<CommentModel>> GetGamesComments(Guid gameId)
    {
        var parentComments = await _appDbContext.Comments.Include(x => x.ParentComment).Where(x => x.Game.Id == gameId && x.ParentComment == null).ToListAsync();

        var parentCommentModels = parentComments.Select(x => new CommentModel(x.Id, x.Name, x.Body, x.Type)).ToList();

        foreach (var parentCommentModel in parentCommentModels)
        {
            parentCommentModel.ChildComments = [];
            await RecursiveGetComments(parentCommentModel, parentCommentModel.ChildComments);
        }

        return parentCommentModels;
    }

    public async Task UpdateComment(Comment comment)
    {
        _appDbContext.Comments.Update(comment);
        await _appDbContext.SaveChangesAsync();
    }

    private async Task RecursiveGetComments(CommentModel comment, List<CommentModel> allComments)
    {
        comment.ChildComments = await _appDbContext.Comments.Include(x => x.ParentComment).Where(x => x.ParentComment.Id == comment.Id).Select(x => new CommentModel(x.Id, x.Name, x.Body, x.Type)).ToListAsync();
        foreach (CommentModel childComment in comment.ChildComments)
        {
            allComments.Add(childComment);
            string currentChildBody = childComment.Body;
            if (childComment.Type == CommentActionType.Reply && currentChildBody != "A comment/quote was deleted")
            {
                childComment.Body = $"[{comment.Name}], {currentChildBody}";
            }
            else if (childComment.Type == CommentActionType.Quote && currentChildBody != "A comment/quote was deleted")
            {
                childComment.ParentQuote = comment.OnlyBody;
                childComment.Body = $"[{childComment.ParentQuote}], {currentChildBody}";
            }

            await RecursiveGetComments(childComment, allComments);
        }
    }
}
