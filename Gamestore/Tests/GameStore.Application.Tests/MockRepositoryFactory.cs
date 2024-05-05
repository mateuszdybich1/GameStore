using GameStore.Domain.IRepositories;
using Moq;

namespace GameStore.Application.Tests;
public class MockRepositoryFactory<T>
    where T : class
{
    public Mock<Func<RepositoryTypes, T>> GetGamesRepository(Mock<T> sqlMock, Mock<T> mongoMock)
    {
        var mockGameRepositoryFactory = new Mock<Func<RepositoryTypes, T>>();

        mockGameRepositoryFactory.Setup(factory => factory(RepositoryTypes.Sql)).Returns(sqlMock.Object);
        mockGameRepositoryFactory.Setup(factory => factory(RepositoryTypes.Mongo)).Returns(mongoMock.Object);

        return mockGameRepositoryFactory;
    }
}
