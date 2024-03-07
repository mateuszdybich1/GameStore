using GameStore.Domain.Entities;
using GameStore.Infrastructure;

namespace GameStore.Web;

internal class PredefinedObjects(AppDbContext appDbContext)
{
    private readonly AppDbContext _appDbContext = appDbContext;

    internal void AddPlatforms()
    {
        List<string> missing = Enum.GetNames(typeof(PlatformType)).Except(_appDbContext.Platforms.Select(x => x.Type)).ToList();

        foreach (var platform in missing)
        {
            _appDbContext.Platforms.Add(new Platform(Guid.NewGuid(), platform));
        }

        _appDbContext.SaveChanges();
    }

    internal void AddGenres()
    {
        var parentGenres = new[] { "Strategy", "RPG", "Sports", "Races", "Action", "Adventure", "Puzzle & Skill" };

        foreach (var parentGenreName in parentGenres)
        {
            Genre genre = _appDbContext.Genres.FirstOrDefault(x => x.Name == parentGenreName);
            if (genre == null)
            {
                genre = new(Guid.NewGuid(), parentGenreName, null, null);
                _appDbContext.Genres.Add(genre);
            }

            switch (parentGenreName)
            {
                case "Strategy":
                    var predefinedStrategyChilds = new[] { "RTS", "TBS" };

                    var strategyChilds = predefinedStrategyChilds.Where(child => !_appDbContext.Genres.Any(x => x.Name == child)).ToList();

                    AddMissingChilds(genre!, strategyChilds);

                    break;

                case "Races":
                    var predefinedRaceChilds = new[] { "Rally", "Arcade", "Formula", "Off-road" };

                    var raceChilds = predefinedRaceChilds.Where(child => !_appDbContext.Genres.Any(x => x.Name == child)).ToList();

                    AddMissingChilds(genre!, raceChilds);

                    break;

                case "Action":
                    var predefinedActionChilds = new[] { "FPS", "TPS" };

                    var actionChilds = predefinedActionChilds.Where(child => !_appDbContext.Genres.Any(x => x.Name == child)).ToList();

                    AddMissingChilds(genre!, actionChilds);

                    break;

                default: break;
            }
        }

        _appDbContext.SaveChanges();
    }

    private void AddMissingChilds(Genre genre, List<string> childs)
    {
        foreach (var missingChild in childs)
        {
            _appDbContext.Genres.Add(new Genre(Guid.NewGuid(), missingChild, null, null, genre));
        }
    }
}