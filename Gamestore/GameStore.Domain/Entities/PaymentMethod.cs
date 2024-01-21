namespace GameStore.Domain.Entities;
public enum PaymentName
{
    Bank = 1,
    IBoxTerminal = 2,
    Visa = 3,
}

public class PaymentMethod
{
    public PaymentMethod()
    {
    }

    public PaymentMethod(string imageUrl, string title, string description)
    {
        ImageUrl = imageUrl;
        Title = title;
        Description = description;
    }

    public string ImageUrl { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }
}
