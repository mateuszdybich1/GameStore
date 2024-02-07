namespace GameStore.Domain.Entities;

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
