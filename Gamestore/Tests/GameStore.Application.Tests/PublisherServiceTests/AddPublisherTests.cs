using GameStore.Application.Dtos;
using GameStore.Application.Services;
using GameStore.Domain;
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
    private readonly Mock<IPublisherRepository> _mongoPublisherRepositoryMock;
    private readonly Mock<IPublisherSearchCriteria> _publisherSearchCriteriaMock;
    private readonly Mock<IPublisherSearchCriteria> _mongoPublisherSearchCriteriaMock;
    private readonly Mock<IChangeLogService> _changeLogServiceMock;

    public PublisherTests()
    {
        _publisherRepositoryMock = new();
        _mongoPublisherRepositoryMock = new();
        Mock<Func<RepositoryTypes, IPublisherRepository>> mockPublisherRepositoryFactory = new MockRepositoryFactory<IPublisherRepository>().GetGamesRepository(_publisherRepositoryMock, _mongoPublisherRepositoryMock);

        _publisherSearchCriteriaMock = new();
        _mongoPublisherSearchCriteriaMock = new();
        Mock<Func<RepositoryTypes, IPublisherSearchCriteria>> mockPublisherSearchCriteriaFactory = new MockRepositoryFactory<IPublisherSearchCriteria>().GetGamesRepository(_publisherSearchCriteriaMock, _mongoPublisherSearchCriteriaMock);

        _changeLogServiceMock = new();
        _publisherService = new(mockPublisherRepositoryFactory.Object, mockPublisherSearchCriteriaFactory.Object, _changeLogServiceMock.Object);
    }

    [Fact]
    public async Task AddPublisherShouldAddPublisherOnce()
    {
        // Arrange
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", "TestHomePage", "TestDescription");
        PublisherDto publisherDto = new(publisher);

        _publisherRepositoryMock.Setup(x => x.Get((Guid)publisherDto.Id!)).Returns(Task.FromResult<Publisher>(null));

        // Act
        Guid publisherId = await _publisherService.AddPublisher(publisherDto);

        // Assert
        Assert.True(publisherId != Guid.Empty);
        _publisherRepositoryMock.Verify(x => x.Add(It.Is<Publisher>(x => x.Id == publisherDto.Id && x.CompanyName == publisherDto.CompanyName)), Times.Once());
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

        _publisherRepositoryMock.Setup(x => x.Get((Guid)publisherDto.Id)).ReturnsAsync(existingPublisher);

        // Assert
        await Assert.ThrowsAsync<ExistingFieldException>(() => _publisherService.AddPublisher(publisherDto));
    }
}
