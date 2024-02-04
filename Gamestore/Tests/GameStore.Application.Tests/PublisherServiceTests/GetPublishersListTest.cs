using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using Xunit;

namespace GameStore.Application.Tests.PublisherServiceTests;
public partial class PublisherTests
{
    [Fact]
    public void GetPublisherListShouldGetList()
    {
        var publishers = new List<Publisher>();

        for (int i = 1; i <= 5; ++i)
        {
            publishers.Add(new Publisher(Guid.NewGuid(), $"CompanyName-{i}", string.Empty, string.Empty));
        }

        _publisherRepositoryMock.Setup(x => x.GetAllPublishers()).Returns(publishers);

        List<PublisherDto> publishersDto = _publisherService.GetAll();

        Assert.Equal(5, publishersDto.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"CompanyName-{i + 1}", publishersDto[i].CompanyName);
        }
    }
}
