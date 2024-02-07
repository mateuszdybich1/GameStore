namespace GameStore.Application.Dtos;

public class PublisherDtoDto(PublisherDto publisherDto)
{
    public PublisherDto Publisher { get; set; } = publisherDto;
}
