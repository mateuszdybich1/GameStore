namespace GameStore.Domain.Entities;
public class Publisher
{
    public Publisher()
    {
    }

    public Publisher(Guid id, string companyName, string homePage, string description)
    {
        Id = id;
        CompanyName = companyName;
        HomePage = homePage;
        Description = description;
    }

    public Guid Id { get; private set; }

    public string CompanyName { get; set; }

    public string HomePage { get; set; }

    public string Description { get; set; }

    public List<Game> Games { get; set; }
}
