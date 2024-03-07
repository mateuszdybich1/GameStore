using GameStore.Domain.Entities;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PublisherServiceTests;
public partial class PublisherTests
{
    [Fact]
    public async Task GetPublisherListShouldGetList()
    {
        var publishers = new List<Publisher>();

        for (int i = 1; i <= 5; ++i)
        {
            publishers.Add(new Publisher(Guid.NewGuid(), $"CompanyName-{i}", string.Empty, string.Empty));
        }

        _repositoryFactory.Setup(x => x("Default").GetAllPublishers()).ReturnsAsync(publishers.AsEnumerable);

        var result = await _publisherService.GetAll();
        var publishersDto = result.ToList();

        Assert.Equal(5, publishersDto.Count);
        for (int i = 0; i < 5; ++i)
        {
            Assert.Equal($"CompanyName-{i + 1}", publishersDto[i].CompanyName);
        }
    }
}
