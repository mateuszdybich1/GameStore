using System.ComponentModel.DataAnnotations;
using GameStore.Domain.Entities;

namespace GameStore.Application.Dtos;

public class PublisherDto
{
    public PublisherDto()
    {
    }

    public PublisherDto(string companyName)
    {
        CompanyName = companyName;
    }

    public PublisherDto(Publisher publisher)
    {
        Id = publisher.Id;
        CompanyName = publisher.CompanyName;
        HomePage = publisher.HomePage;
        Description = publisher.Description;
    }

    public Guid? Id { get; set; }

    [Required]
    public string CompanyName { get; set; }

    public string? HomePage { get; set; }

    public string? Description { get; set; }
}
