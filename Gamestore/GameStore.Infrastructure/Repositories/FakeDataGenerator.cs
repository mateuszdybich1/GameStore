using System.Diagnostics;
using Bogus;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;

namespace GameStore.Infrastructure.Repositories;

public class FakeDataGenerator(AppDbContext appDbContext) : IFakeDataGenerator
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task Add100kGames()
    {
        var watch = Stopwatch.StartNew();
        var publisherId = Guid.NewGuid();
        var publisher = new Publisher(publisherId, "RandomPublisherName", string.Empty, string.Empty);
        var genre = new Genre(Guid.NewGuid(), "RandomGenreName", null, null);
        var platform = new Platform(Guid.NewGuid(), "RandomPlatformName");
        _appDbContext.Publishers.Add(publisher);
        _appDbContext.Genres.Add(genre);
        _appDbContext.Platforms.Add(platform);

        await _appDbContext.SaveChangesAsync();

        var genreList = new List<Genre>() { genre };
        var platformList = new List<Platform>() { platform };

        var faker = new Faker<Game>()
            .RuleFor(g => g.Id, f => f.Commerce.Random.Uuid())
            .RuleFor(g => g.Name, f => f.Commerce.ProductName())
            .RuleFor(g => g.Key, f => f.Random.AlphaNumeric(10))
            .RuleFor(g => g.Description, f => f.Lorem.Sentence())
            .RuleFor(g => g.Price, f => f.Random.Double(1, 1000))
            .RuleFor(g => g.UnitInStock, f => f.Random.Int(0, 100))
            .RuleFor(g => g.Discount, f => f.Random.Double(0, 1))
            .RuleFor(g => g.NumberOfViews, f => f.Random.ULong(0, 1000000))
            .RuleFor(g => g.PublisherId, publisherId)
            .RuleFor(g => g.Publisher, publisher)
            .RuleFor(g => g.Genres, genreList)
            .RuleFor(g => g.Platforms, platformList);

        var games = faker.Generate(100000);

        _appDbContext.Games.AddRange(games);
        await _appDbContext.SaveChangesAsync();

        watch.Stop();

        Debug.WriteLine($"100k games time: {watch.ElapsedMilliseconds} ms");
    }
}
