namespace GameStore.Domain.Entities;
public class Publisher : Entity
{
    public Publisher()
    {
    }

    public Publisher(Guid id, string companyName, string homePage, string description)
        : base(id)
    {
        CompanyName = companyName;
        HomePage = homePage;
        Description = description;
    }

    public string CompanyName { get; set; }

    public string HomePage { get; set; }

    public string Description { get; set; }

    public List<Game> Games { get; set; }
}
