namespace GameStore.Domain.Entities;

public enum CommentActionType
{
    Normal,
    Reply,
    Quote,
}

public class Comment : Entity
{
    public Comment()
    {
    }

    public Comment(Guid id, string name, string body, CommentActionType type, Game game)
        : base(id)
    {
        Name = name;
        Body = body;
        Type = type;
        Game = game;
    }

    public Comment(Guid id, string name, string body, CommentActionType type, Game game, Comment parentComment)
        : base(id)
    {
        Name = name;
        Body = body;
        Type = type;
        Game = game;
        ParentComment = parentComment;
    }

    public Comment(Comment comment)
        : base(comment.Id, comment.CreationDate, comment.ModificationDate)
    {
        Name = comment.Name;
        Body = comment.Body;
        Type = comment.Type;
        Game = comment.Game;
        ParentComment = comment.ParentComment;
    }

    public string Name { get; set; }

    public string Body { get; set; }

    public CommentActionType Type { get; set; }

    public Game Game { get; set; }

    public Comment? ParentComment { get; set; }
}
