using GameStore.Domain.MongoEntities;

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

    public Publisher(MongoPublisher supplier)
        : base(supplier.Id.AsGuid())
    {
        CompanyName = (string)supplier.CompanyName;
        HomePage = string.IsNullOrEmpty((string)supplier.HomePage) ? string.Empty : (string)supplier.HomePage;

        var descr = string.Empty;
        if (supplier.ContactName != null && (supplier.ContactName is int || supplier.ContactName != string.Empty))
        {
            descr += $"ContactName: {supplier.ContactName}; ";
        }

        if (supplier.ContactTitle != null && (supplier.ContactTitle is int || supplier.ContactTitle != string.Empty))
        {
            descr += $"ContactTitle: {supplier.ContactTitle}; ";
        }

        if (supplier.Address != null && (supplier.Address is int || supplier.Address != string.Empty))
        {
            descr += $"Address: {supplier.Address}; ";
        }

        if (supplier.City != null && (supplier.City is int || supplier.City != string.Empty))
        {
            descr += $"City: {supplier.City}; ";
        }

        if (supplier.Region != null && (supplier.Region is int || supplier.Region != string.Empty))
        {
            descr += $"Region: {supplier.Region}; ";
        }

        if (supplier.PostalCode != null && (supplier.PostalCode is int || supplier.PostalCode != string.Empty))
        {
            descr += $"PostalCode: {supplier.PostalCode}; ";
        }

        if (supplier.Country != null && (supplier.Country is int || supplier.Country != string.Empty))
        {
            descr += $"Country: {supplier.Country}; ";
        }

        if (supplier.Phone != null && (supplier.Phone is int || supplier.Phone != string.Empty))
        {
            descr += $"Phone: {supplier.Phone}; ";
        }

        if (supplier.Fax != null && (supplier.Fax is int || supplier.Fax != string.Empty))
        {
            descr += $"Fax: {supplier.Fax}; ";
        }

        Description = descr.ToString();
    }

    public string CompanyName { get; set; }

    public string HomePage { get; set; }

    public string Description { get; set; }

    public List<Game> Games { get; set; }
}
