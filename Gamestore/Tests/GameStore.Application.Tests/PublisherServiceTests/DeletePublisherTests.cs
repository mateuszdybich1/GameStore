using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PublisherServiceTests;
public partial class PublisherTests
{
    [Fact]
    public void DeletePublisherShouldDeletePublisherOnce()
    {
        // Arrange
        Guid publisherId = Guid.NewGuid();
        Publisher publisher = new(publisherId, "TestCompanyName", string.Empty, string.Empty);

        _publisherRepositoryMock.Setup(x => x.GetPublisher(publisherId)).Returns(publisher);

        // Act
        Guid deletedPublisherId = _publisherService.DeletePublisher(publisherId);

        // Assert
        Assert.Equal(publisherId, deletedPublisherId);
        _publisherRepositoryMock.Verify(x => x.GetPublisher(publisherId), Times.Once);
        _publisherRepositoryMock.Verify(x => x.DeletePublisher(It.Is<Publisher>(x => x == publisher)), Times.Once);
    }

    [Fact]
    public void DeletePublisherIncorrectPublisherIdProvidedShouldThrowException()
    {
        // Arrange
        Guid publisherId = Guid.NewGuid();
        _publisherRepositoryMock.Setup(x => x.GetPublisher(publisherId)).Returns((Publisher)null);

        // Act and Assert
        Assert.Throws<EntityNotFoundException>(() => _publisherService.DeletePublisher(publisherId));
    }
}
