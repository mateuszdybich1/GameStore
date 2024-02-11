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

    public Comment(Guid id, string name, string body, Game game, Comment parentComment)
        : base(id)
    {
        Name = name;
        Body = body;
        Game = game;
        ParentComment = parentComment;
    }

    public string Name { get; set; }

    public string Body { get; set; }

    public CommentActionType Type { get; set; }

    public Game Game { get; set; }

    public Comment? ParentComment { get; set; }
}
