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
    private readonly Mock<Func<string, IPublisherRepository>> _repositoryFactory;
    private readonly Mock<IPublisherSearchCriteria> _publisherSearchCriteriaMock;

    public PublisherTests()
    {
        _publisherSearchCriteriaMock = new();

        _publisherService = new(_repositoryFactory.Object, _publisherSearchCriteriaMock.Object);
    }

    [Fact]
    public async Task AddPublisherShouldAddPublisherOnce()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", "TestHomePage", "TestDescription");
        PublisherDto publisherDto = new(publisher);

        // Act
        Guid publisherId = await _publisherService.AddPublisher(publisherDto);

        // Assert
        Assert.True(publisherId != Guid.Empty);
        _repositoryFactory.Verify(x => x("Default").Add(It.Is<Publisher>(x => x.Id == publisherDto.Id && x.CompanyName == publisherDto.CompanyName)), Times.Once());
    }

    [Fact]
    public async Task AddPublisherIncorrectPublisherIdProvidedShouldThrowException()
    {
        // Arrange
        PublisherDto publisherDto = new("TestCompany")
        {
            Id = Guid.NewGuid(),
        };

        // Act
        Publisher existingPublisher = new((Guid)publisherDto.Id, "ExistingCompany", string.Empty, string.Empty);

        _repositoryFactory.Setup(x => x("Default").Get((Guid)publisherDto.Id)).ReturnsAsync(existingPublisher);

        // Assert
        await Assert.ThrowsAsync<ExistingFieldException>(() => _publisherService.AddPublisher(publisherDto));
    }
}
