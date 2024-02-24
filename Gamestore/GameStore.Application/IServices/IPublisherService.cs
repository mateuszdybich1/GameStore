using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IPublisherService
{
    public Task<Guid> AddPublisher(PublisherDto publisherDto);

    public Task<PublisherDto> GetPublisherByCompanyName(string companyName);

    public Task<PublisherDto> GetPublisherByGameKey(string gameKey);

    public Task<Guid> UpdatePublisher(PublisherDto publisherDto);

    public Task<Guid> DeletePublisher(Guid publisherId);

    public Task<IEnumerable<PublisherDto>> GetAll();
}
