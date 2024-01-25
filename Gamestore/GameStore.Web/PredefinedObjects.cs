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

        List<string> missingParents = parentGenres.Except(_appDbContext.Genres.Select(x => x.Name)).ToList();

        foreach (var parentGenre in parentGenres)
        {
            Genre genre;
            if (!_appDbContext.Genres.Select(x => x.Name).Contains(parentGenre))
            {
                genre = new(Guid.NewGuid(), parentGenre);
                _appDbContext.Genres.Add(genre);
            }
            else
            {
                genre = _appDbContext.Genres.FirstOrDefault(x => x.Name == parentGenre);
            }

            switch (parentGenre)
            {
                case "Strategy":
                    var predefinedStrategyChilds = new[] { "RTS", "TBS" };

                    var strategyChilds = predefinedStrategyChilds.Where(child => !_appDbContext.Genres.Any(x => x.Name == child)).ToList();

                    AddMissingChilds(genre.Id, strategyChilds);

                    break;

                case "Races":
                    var predefinedRaceChilds = new[] { "Rally", "Arcade", "Formula", "Off-road" };

                    var raceChilds = predefinedRaceChilds.Where(child => !_appDbContext.Genres.Any(x => x.Name == child)).ToList();

                    AddMissingChilds(genre.Id, raceChilds);

                    break;

                case "Action":
                    var predefinedActionChilds = new[] { "FPS", "TPS" };

                    var actionChilds = predefinedActionChilds.Where(child => !_appDbContext.Genres.Any(x => x.Name == child)).ToList();

                    AddMissingChilds(genre.Id, actionChilds);

                    break;

                default: break;
            }
        }

        _appDbContext.SaveChanges();
    }

    private void AddMissingChilds(Guid parentId, List<string> childs)
    {
        foreach (var missingChild in childs)
        {
            _appDbContext.Genres.Add(new Genre(Guid.NewGuid(), missingChild, parentId));
        }
    }
}