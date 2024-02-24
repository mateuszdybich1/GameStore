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

    public async Task<Guid> AddPublisher(PublisherDto publisherDto)
    {
        if (publisherDto.Id != null && await _publisherRepository.Get((Guid)publisherDto.Id) != null)
        {
            throw new ExistingFieldException($"Publisher with ID: {publisherDto.Id} already exists");
        }

        Guid publisherId = (publisherDto.Id == null || publisherDto.Id == Guid.Empty) ? Guid.NewGuid() : (Guid)publisherDto.Id;

        string homePage = publisherDto.HomePage ?? string.Empty;
        string description = publisherDto.Description ?? string.Empty;

        Publisher publisher = new(publisherId, publisherDto.CompanyName, homePage, description);

        try
        {
            await _publisherRepository.Add(publisher);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique company name");
        }

        return publisher.Id;
    }

    public async Task<Guid> DeletePublisher(Guid publisherId)
    {
        Publisher publisher = await _publisherRepository.Get(publisherId) ?? throw new EntityNotFoundException($"Couldn't find publisher by ID: {publisherId}");

        await _publisherRepository.Delete(publisher);

        return publisher.Id;
    }

    public async Task<IEnumerable<PublisherDto>> GetAll()
    {
        var publishers = await _publisherRepository.GetAllPublishers();
        return publishers.Select(x => new PublisherDto(x));
    }

    public async Task<PublisherDto> GetPublisherByCompanyName(string companyName)
    {
        Publisher publisher = await _publisherSearchCriteria.GetPublisherByCompanyName(companyName) ?? throw new EntityNotFoundException($"Couldn't find publisher by company name: {companyName}");

        return new(publisher);
    }

    public async Task<PublisherDto> GetPublisherByGameKey(string gameKey)
    {
        Publisher publisher = await _publisherSearchCriteria.GetPublisherByGameKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find publisher by game key: {gameKey}");

        return new(publisher);
    }

    public async Task<Guid> UpdatePublisher(PublisherDto publisherDto)
    {
        if (publisherDto.Id == null)
        {
            throw new ArgumentNullException("Cannot update publisher. Id is null");
        }

        Publisher publisher = await _publisherRepository.Get((Guid)publisherDto.Id) ?? throw new EntityNotFoundException($"Couldn't find publisher by ID: {publisherDto.Id}");

        publisher.CompanyName = publisherDto.CompanyName;
        publisher.HomePage = publisherDto.HomePage ?? string.Empty;
        publisher.Description = publisherDto.Description ?? string.Empty;
        publisher.ModificationDate = DateTime.Now;

        try
        {
            await _publisherRepository.Update(publisher);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique company name");
        }

        return publisher.Id;
    }
}
