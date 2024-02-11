namespace GameStore.Domain.Entities;

public class CommentModel(Guid id, string name, string body, CommentActionType type) : Entity(id)
{
    public string Name { get; set; } = name;

    public string? ParentQuote { get; set; }

    public string OnlyBody { get; set; } = body;

    public string Body { get; set; } = body;

    public CommentActionType Type { get; set; } = type;

    public List<CommentModel> ChildComments { get; set; }
}
