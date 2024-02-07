using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PublisherServiceTests;
public partial class PublisherTests
{
    [Fact]
    public void UpdatePublisherShouldUpdatePublisherOnce()
    {
        // Arrange
        Guid publisherId = Guid.NewGuid();

        Publisher publisher = new(publisherId, "TestCompany", string.Empty, string.Empty);
        Publisher updatedPublisher = new(publisherId, "UpdatedCompany", "TestHomePage", "TestDescription");
        PublisherDto publisherDto = new(updatedPublisher);

        _publisherRepositoryMock.Setup(x => x.GetPublisher(publisherId)).Returns(publisher);

        // Act
        _publisherService.UpdatePublisher(publisherDto);

        // Assert
        _publisherRepositoryMock.Verify(x => x.GetPublisher(publisherId), Times.Once);
        _publisherRepositoryMock.Verify(x => x.UpdatePublisher(It.Is<Publisher>(x => x.Id == publisherId && x.CompanyName == publisherDto.CompanyName && x.HomePage == publisherDto.HomePage && x.Description == publisherDto.Description)), Times.Once);
    }

    [Fact]
    public void UpdatePublisherPublisherIdNotProvidedShouldThrowException()
    {
        // Arrange
        PublisherDto publisherDto = new()
        {
            Id = null,
            CompanyName = "TestCompany",
        };

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => _publisherService.UpdatePublisher(publisherDto));
    }
}
