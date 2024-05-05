using GameStore.Application.Dtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Application.Tests.PublisherServiceTests;
public partial class PublisherTests
{
    [Fact]
    public async Task GetPublisherByCompanyNameShouldGetPublisherDto()
    {
        // Arrange
        string companyName = "TestCompany";

        Publisher publisher = new(Guid.NewGuid(), companyName, string.Empty, string.Empty);
        _publisherSearchCriteriaMock.Setup(x => x.GetPublisherByCompanyName(companyName)).ReturnsAsync(publisher);
        _mongoPublisherSearchCriteriaMock.Setup(x => x.GetPublisherByCompanyName(companyName)).Returns(Task.FromResult<Publisher>(null));

        // Act
        PublisherDto publisherDto = await _publisherService.GetPublisherByCompanyName(companyName);

        // Assert
        Assert.NotNull(publisherDto);
        _publisherSearchCriteriaMock.Verify(x => x.GetPublisherByCompanyName(companyName), Times.Once);
        Assert.True(publisher.Id == publisherDto.Id &&
                    publisher.CompanyName == publisherDto.CompanyName &&
                    publisher.HomePage == publisherDto.HomePage &&
                    publisher.Description == publisherDto.Description);
    }

    [Fact]
    public async Task GetPublisherByCompanyNameIncorrectCompanyNameProvidedShouldThrowException()
    {
        // Arrange
        string companyName = "RandomCompanyName";
        _publisherSearchCriteriaMock.Setup(x => x.GetPublisherByCompanyName(companyName)).Returns(Task.FromResult<Publisher>(null));
        _mongoPublisherSearchCriteriaMock.Setup(x => x.GetPublisherByCompanyName(companyName)).Returns(Task.FromResult<Publisher>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _publisherService.GetPublisherByCompanyName(companyName));
    }

    [Fact]
    public async Task GetPublisherByGameKeyShouldGetPublisherDto()
    {
        // Arrange
        string gameKey = "TestKey";
        Publisher publisher = new(Guid.NewGuid(), "TestCompany", string.Empty, string.Empty);

        _publisherSearchCriteriaMock.Setup(x => x.GetPublisherByGameKey(gameKey)).ReturnsAsync(publisher);
        _mongoPublisherSearchCriteriaMock.Setup(x => x.GetPublisherByGameKey(gameKey)).Returns(Task.FromResult<Publisher>(null));

        // Act
        PublisherDto publisherDto = await _publisherService.GetPublisherByGameKey(gameKey);

        // Assert
        Assert.NotNull(publisherDto);
        _publisherSearchCriteriaMock.Verify(x => x.GetPublisherByGameKey(gameKey), Times.Once);
        Assert.True(publisher.Id == publisherDto.Id &&
                    publisher.CompanyName == publisherDto.CompanyName &&
                    publisher.HomePage == publisherDto.HomePage &&
                    publisher.Description == publisherDto.Description);
    }

    [Fact]
    public async Task GetPublisherByGameKeyIncorrectGameKeyProvidedShouldThrowException()
    {
        // Arrange
        string gameKey = "TestKey";
        _publisherSearchCriteriaMock.Setup(x => x.GetPublisherByGameKey(gameKey)).Returns(Task.FromResult<Publisher>(null));
        _mongoPublisherSearchCriteriaMock.Setup(x => x.GetPublisherByGameKey(gameKey)).Returns(Task.FromResult<Publisher>(null));

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _publisherService.GetPublisherByCompanyName(gameKey));
    }
}
