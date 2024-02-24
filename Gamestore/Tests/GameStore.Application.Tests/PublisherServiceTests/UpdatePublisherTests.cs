using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PublisherServiceTests;
public partial class PublisherTests
{
    [Fact]
    public async Task UpdatePublisherShouldUpdatePublisherOnce()
    {
        // Arrange
        Guid publisherId = Guid.NewGuid();

        Publisher publisher = new(publisherId, "TestCompany", string.Empty, string.Empty);
        Publisher updatedPublisher = new(publisherId, "UpdatedCompany", "TestHomePage", "TestDescription");
        PublisherDto publisherDto = new(updatedPublisher);

        _publisherRepositoryMock.Setup(x => x.Get(publisherId)).ReturnsAsync(publisher);

        // Act
        await _publisherService.UpdatePublisher(publisherDto);

        // Assert
        _publisherRepositoryMock.Verify(x => x.Get(publisherId), Times.Once);
        _publisherRepositoryMock.Verify(x => x.Update(It.Is<Publisher>(x => x.Id == publisherId && x.CompanyName == publisherDto.CompanyName && x.HomePage == publisherDto.HomePage && x.Description == publisherDto.Description)), Times.Once);
    }

    [Fact]
    public async Task UpdatePublisherPublisherIdNotProvidedShouldThrowException()
    {
        // Arrange
        PublisherDto publisherDto = new()
        {
            Id = null,
            CompanyName = "TestCompany",
        };

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _publisherService.UpdatePublisher(publisherDto));
    }
}
