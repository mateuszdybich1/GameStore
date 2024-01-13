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

        foreach (var parentGenre in missingParents)
        {
            Genre genre = new(Guid.NewGuid(), parentGenre);
            _appDbContext.Genres.Add(genre);

            switch (parentGenre)
            {
                case "Strategy":
                    List<string> strategyChilds = new([.. _appDbContext.Genres.Where(y => y.ParentGerneId == genre.Id).Select(x => x.Name)]);

                    var predefinedStrategyChilds = new[] { "RTS", "TBS" };

                    AddMissingChilds(genre.Id, strategyChilds, predefinedStrategyChilds);

                    break;

                case "Races":
                    List<string> raceChilds = new([.. _appDbContext.Genres.Where(y => y.ParentGerneId == genre.Id).Select(x => x.Name)]);

                    var predefinedRaceChilds = new[] { "Rally", "Arcade", "Formula", "Off-road" };

                    AddMissingChilds(genre.Id, raceChilds, predefinedRaceChilds);

                    break;

                case "Action":
                    List<string> actionChilds = new([.. _appDbContext.Genres.Where(y => y.ParentGerneId == genre.Id).Select(x => x.Name)]);

                    var predefinedActionChilds = new[] { "FPS", "TPS" };

                    AddMissingChilds(genre.Id, actionChilds, predefinedActionChilds);

                    break;

                default: break;
            }
        }

        _appDbContext.SaveChanges();
    }

    private void AddMissingChilds(Guid parentId, List<string> allChilds, string[] predefinedChilds)
    {
        List<string> missingChilds = predefinedChilds.Except(allChilds).ToList();

        foreach (var missingChild in missingChilds)
        {
            _appDbContext.Genres.Add(new Genre(Guid.NewGuid(), missingChild, parentId));
        }
    }
}