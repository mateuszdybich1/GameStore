using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IPublisherService
{
    Task<Guid> AddPublisher(PublisherDto publisherDto);

    Task<PublisherDto> GetPublisherByCompanyName(string companyName);

    Task<PublisherDto> GetPublisherByGameKey(string gameKey);

    Task<Guid> UpdatePublisher(PublisherDto publisherDto);

    Task<Guid> DeletePublisher(Guid publisherId);

    Task<IEnumerable<PublisherDto>> GetAll();
}
