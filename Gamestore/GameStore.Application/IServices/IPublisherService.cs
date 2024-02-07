using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IPublisherService
{
    public Guid AddPublisher(PublisherDto publisherDto);

    public PublisherDto GetPublisherByCompanyName(string companyName);

    public PublisherDto GetPublisherByGameKey(string gameKey);

    public Guid UpdatePublisher(PublisherDto publisherDto);

    public Guid DeletePublisher(Guid publisherId);

    List<PublisherDto> GetAll();
}
