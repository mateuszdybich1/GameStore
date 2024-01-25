using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class PublisherService(IPublisherRepository publisherRepository, IPublisherSearchCriteria publisherSearchCriteria) : IPublisherService
{
    private readonly IPublisherRepository _publisherRepository = publisherRepository;
    private readonly IPublisherSearchCriteria _publisherSearchCriteria = publisherSearchCriteria;

    public Guid AddPublisher(PublisherDto publisherDto)
    {
        Guid publisherId = (publisherDto.Id == null || publisherDto.Id == Guid.Empty) ? Guid.NewGuid() : (Guid)publisherDto.Id;

        string homePage = publisherDto.HomePage ?? string.Empty;
        string description = publisherDto.Description ?? string.Empty;

        Publisher publisher = new(publisherId, publisherDto.CompanyName, homePage, description);

        try
        {
            _publisherRepository.AddPublisher(publisher);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique company name");
        }

        return publisher.Id;
    }

    public Guid DeletePublisher(Guid publisherId)
    {
        Publisher publisher = _publisherRepository.GetPublisher(publisherId) ?? throw new EntityNotFoundException($"Couldn't find publisher by ID: {publisherId}");

        _publisherRepository.DeletePublisher(publisher);

        return publisher.Id;
    }

    public List<PublisherDto> GetAll()
    {
        return _publisherRepository.GetAllPublishers().Select(x => new PublisherDto(x)).ToList();
    }

    public PublisherDto GetPublisherByCompanyName(string companyName)
    {
        Publisher publisher = _publisherSearchCriteria.GetPublisherByCompanyName(companyName) ?? throw new EntityNotFoundException($"Couldn't find publisher by company name: {companyName}");

        return new(publisher);
    }

    public PublisherDto GetPublisherByGameKey(string gameKey)
    {
        Publisher publisher = _publisherSearchCriteria.GetPublisherByGameKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find publisher by game key: {gameKey}");

        return new(publisher);
    }

    public Guid UpdatePublisher(PublisherDto publisherDto)
    {
        if (publisherDto.Id == null)
        {
            throw new ArgumentNullException("Cannot update publisher. Id is null");
        }

        Publisher publisher = _publisherRepository.GetPublisher((Guid)publisherDto.Id) ?? throw new EntityNotFoundException($"Couldn't find publisher by ID: {publisherDto.Id}");

        publisher.CompanyName = publisherDto.CompanyName;
        publisher.HomePage = publisherDto.HomePage ?? string.Empty;
        publisher.Description = publisherDto.Description ?? string.Empty;

        try
        {
            _publisherRepository.UpdatePublisher(publisher);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique company name");
        }

        return publisher.Id;
    }
}
