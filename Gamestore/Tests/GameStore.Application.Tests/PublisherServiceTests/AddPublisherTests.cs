using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PublisherServiceTests;
public partial class PublisherTests
{
    private readonly PublisherService _publisherService;
    private readonly Mock<IPublisherRepository> _publisherRepositoryMock;
    private readonly Mock<IPublisherSearchCriteria> _publisherSearchCriteriaMock;

    public PublisherTests()
    {
        _publisherRepositoryMock = new();
        _publisherSearchCriteriaMock = new();

        _publisherService = new(_publisherRepositoryMock.Object, _publisherSearchCriteriaMock.Object);
    }

    [Fact]
    public void AddPublisherShouldAddPublisherOnce()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", "TestHomePage", "TestDescription");
        PublisherDto publisherDto = new(publisher);

        // Act
        Guid publisherId = _publisherService.AddPublisher(publisherDto);

        // Assert
        Assert.True(publisherId != Guid.Empty);
        _publisherRepositoryMock.Verify(x => x.AddPublisher(It.Is<Publisher>(x => x.Id == publisherDto.Id && x.CompanyName == publisherDto.CompanyName)), Times.Once());
    }

    [Fact]
    public void AddPublisherIncorrectPublisherIdProvidedShouldThrowException()
    {
        // Arrange
        PublisherDto publisherDto = new("TestCompany")
        {
            Id = Guid.NewGuid(),
        };

        // Act
        Publisher existingPublisher = new((Guid)publisherDto.Id, "ExistingCompany", string.Empty, string.Empty);

        _publisherRepositoryMock.Setup(x => x.GetPublisher((Guid)publisherDto.Id)).Returns(existingPublisher);

        // Assert
        Assert.Throws<ExistingFieldException>(() => _publisherService.AddPublisher(publisherDto));
    }
}
