using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class PublisherService(Func<RepositoryTypes, IPublisherRepository> publisherFactory, Func<RepositoryTypes, IPublisherSearchCriteria> criteriaFactory) : IPublisherService
{
    private readonly IPublisherSearchCriteria _sqlPublisherSearchCriteria = criteriaFactory(RepositoryTypes.Sql);
    private readonly IPublisherSearchCriteria _mongoPublisherSearchCriteria = criteriaFactory(RepositoryTypes.Mongo);
    private readonly IPublisherRepository _sqlPublisherRepository = publisherFactory(RepositoryTypes.Sql);
    private readonly IPublisherRepository _mongoPublisherRepository = publisherFactory(RepositoryTypes.Mongo);

    public async Task<Guid> AddPublisher(PublisherDto publisherDto)
    {
        if (publisherDto.Id != null && await _sqlPublisherRepository.Get((Guid)publisherDto.Id) != null)
        {
            throw new ExistingFieldException($"Publisher with ID: {publisherDto.Id} already exists");
        }

        Guid publisherId = (publisherDto.Id == null || publisherDto.Id == Guid.Empty) ? Guid.NewGuid() : (Guid)publisherDto.Id;

        string homePage = publisherDto.HomePage ?? string.Empty;
        string description = publisherDto.Description ?? string.Empty;

        Publisher publisher = new(publisherId, publisherDto.CompanyName, homePage, description);

        try
        {
            await _sqlPublisherRepository.Add(publisher);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique company name");
        }

        return publisher.Id;
    }

    public async Task<Guid> DeletePublisher(Guid publisherId)
    {
        Publisher publisher = await _sqlPublisherRepository.Get(publisherId);
        var mongoPublisher = await _mongoPublisherRepository.Get(publisherId);

        if (mongoPublisher == null && publisher == null)
        {
            throw new EntityNotFoundException($"Couldn't find publisher by ID: {publisherId}");
        }
        else
        {
            if (mongoPublisher != null)
            {
                await _mongoPublisherRepository.Delete(mongoPublisher);
            }

            if (publisher != null)
            {
                await _sqlPublisherRepository.Delete(publisher);
            }
        }

        return publisher == null ? mongoPublisher.Id : publisher.Id;
    }

    public async Task<IEnumerable<PublisherDto>> GetAll()
    {
        var publishers = await _sqlPublisherRepository.GetAllPublishers();
        var mongoPublishers = await _mongoPublisherRepository.GetAllPublishers();
        mongoPublishers = mongoPublishers.Where(x => !publishers.Any(y => y.CompanyName == x.CompanyName));
        return publishers.Concat(mongoPublishers).Select(x => new PublisherDto(x));
    }

    public async Task<PublisherDto> GetPublisherByCompanyName(string companyName)
    {
        Publisher publisher = await _sqlPublisherSearchCriteria.GetPublisherByCompanyName(companyName);
        publisher ??= await _mongoPublisherSearchCriteria.GetPublisherByCompanyName(companyName) ?? throw new EntityNotFoundException($"Couldn't find publisher by company name: {companyName}");

        return new(publisher);
    }

    public async Task<PublisherDto> GetPublisherByGameKey(string gameKey)
    {
        Publisher publisher = await _sqlPublisherSearchCriteria.GetPublisherByGameKey(gameKey);
        publisher ??= await _mongoPublisherSearchCriteria.GetPublisherByGameKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find publisher by game key: {gameKey}");

        return new(publisher);
    }

    public async Task<Guid> UpdatePublisher(PublisherDto publisherDto)
    {
        if (publisherDto.Id == null)
        {
            throw new ArgumentNullException("Cannot update publisher. Id is null");
        }

        Publisher publisher = await _sqlPublisherRepository.Get((Guid)publisherDto.Id);
        Publisher mongoPublisher = await _mongoPublisherRepository.Get((Guid)publisherDto.Id);

        bool sameCompanyname;

        if (publisher == null && mongoPublisher == null)
        {
            throw new EntityNotFoundException($"Couldn't find publisher by ID: {publisherDto.Id}");
        }
        else
        {
            sameCompanyname = publisher != null ? publisher.CompanyName == publisherDto.CompanyName : mongoPublisher.CompanyName == publisherDto.CompanyName;
            if (publisher != null)
            {
                publisher.CompanyName = publisherDto.CompanyName;
                publisher.HomePage = publisherDto.HomePage ?? string.Empty;
                publisher.Description = publisherDto.Description ?? string.Empty;
                publisher.ModificationDate = DateTime.Now;
            }

            if (mongoPublisher != null)
            {
                mongoPublisher.CompanyName = publisherDto.CompanyName;
                mongoPublisher.HomePage = publisherDto.HomePage ?? string.Empty;
                mongoPublisher.Description = publisherDto.Description ?? string.Empty;
                mongoPublisher.ModificationDate = DateTime.Now;
            }
        }

        try
        {
            if (mongoPublisher != null && publisher != null)
            {
                await MongoUpdate(mongoPublisher, sameCompanyname);
                await _sqlPublisherRepository.Update(publisher);
            }
            else
            {
                if (mongoPublisher != null && publisher == null)
                {
                    await MongoUpdate(mongoPublisher, sameCompanyname);
                    await _sqlPublisherRepository.Add(mongoPublisher);
                }
                else if (mongoPublisher == null && publisher != null)
                {
                    await _sqlPublisherRepository.Update(publisher);
                }
            }
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique company name");
        }

        return publisher == null ? mongoPublisher.Id : publisher.Id;
    }

    private async Task MongoUpdate(Publisher publisher, bool sameCompanyName)
    {
        if (!sameCompanyName)
        {
            Publisher mongoPublisher = await _mongoPublisherSearchCriteria.GetPublisherByCompanyName(publisher.CompanyName);

            if (mongoPublisher != null)
            {
                throw new ExistingFieldException($"Publisher with Company name: {publisher.CompanyName} already exists");
            }

            await _mongoPublisherRepository.Update(publisher);
        }
        else
        {
            await _mongoPublisherRepository.Update(publisher);
        }
    }
}
