using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PublisherServiceTests;
public partial class PublisherTests
{
    [Fact]
    public async Task DeletePublisherShouldDeletePublisherOnce()
    {
        // Arrange
        Guid publisherId = Guid.NewGuid();
        Publisher publisher = new(publisherId, "TestCompanyName", string.Empty, string.Empty);

        _repositoryFactory.Setup(x => x("Default").Get(publisherId)).ReturnsAsync(publisher);

        // Act
        Guid deletedPublisherId = await _publisherService.DeletePublisher(publisherId);

        // Assert
        Assert.Equal(publisherId, deletedPublisherId);
        _repositoryFactory.Verify(x => x("Default").Get(publisherId), Times.Once);
        _repositoryFactory.Verify(x => x("Default").Delete(It.Is<Publisher>(x => x == publisher)), Times.Once);
    }

    [Fact]
    public async Task DeletePublisherIncorrectPublisherIdProvidedShouldThrowException()
    {
        // Arrange
        Guid publisherId = Guid.NewGuid();
        _repositoryFactory.Setup(x => x("Default").Get(publisherId)).Returns(Task.FromResult<Publisher>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _publisherService.DeletePublisher(publisherId));
    }
}
