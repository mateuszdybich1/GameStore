using GameStore.Infrastructure.IRepositories;
using GameStore.Web.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GameStore.Web.Tests;

public class MiddlewareUnitTests
{
    private readonly Mock<IGameRepository> _gameRepositoryMock;

    private readonly DefaultHttpContext _httpContext;

    public MiddlewareUnitTests()
    {
        _gameRepositoryMock = new Mock<IGameRepository>();
        _httpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task TotalGamesShouldSetHeader()
    {
        // Arrange
        _gameRepositoryMock.Setup(x => x.GetAllGames()).Returns([new(), new(), new()]);

        var next = new RequestDelegate(innerContext => Task.CompletedTask);

        _httpContext.Response.Body = new MemoryStream();
        var middleware = new TotalGamesMiddleware(next);

        // Act
        await middleware.InvokeAsync(_httpContext, _gameRepositoryMock.Object);
        await _httpContext.Response.StartAsync();

        // Assert
        Assert.True(_httpContext.Response.Headers.ContainsKey("x-total-numbers-of-games"));
        Assert.Equal("3", _httpContext.Response.Headers["x-total-numbers-of-games"]);
    }
}
