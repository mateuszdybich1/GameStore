using GameStore.Infrastructure;
using GameStore.Infrastructure.Entities;

namespace GameStore.Web;

internal class PredefinedObjects
{
    public PredefinedObjects(AppDbContext context)
    {
        Context = context;
    }

    internal AppDbContext Context { get; private set; }

    internal void AddPlatforms()
    {
        List<string> missing = Enum.GetNames(typeof(PlatformType)).Except(Context.Platforms.Select(x => x.Type)).ToList();

        foreach (var platform in missing)
        {
            Context.Platforms.Add(new Platform(Guid.NewGuid(), platform));
        }

        Context.SaveChanges();
    }

    internal void AddGenres()
    {
        string[] parentGenres = { "Strategy", "RPG", "Sports", "Races", "Action", "Adventure", "Puzzle & Skill" };

        List<string> missingParents = parentGenres.Except(Context.Genres.Select(x => x.Name)).ToList();

        foreach (var parentGenre in missingParents)
        {
            Genre genre = new(Guid.NewGuid(), parentGenre);
            Context.Genres.Add(genre);

            switch (parentGenre)
            {
                case "Strategy":
                    List<string> strategyChilds = Context.Genres.Where(y => y.ParentGerneId == genre.Id).Select(x => x.Name).ToList();

                    string[] predefinedStrategyChilds = { "RTS", "TBS" };

                    AddMissingChilds(genre.Id, strategyChilds, predefinedStrategyChilds);

                    break;

                case "Races":
                    List<string> raceChilds = Context.Genres.Where(y => y.ParentGerneId == genre.Id).Select(x => x.Name).ToList();

                    string[] predefinedRaceChilds = { "Rally", "Arcade", "Formula", "Off-road" };

                    AddMissingChilds(genre.Id, raceChilds, predefinedRaceChilds);

                    break;

                case "Action":
                    List<string> actionChilds = Context.Genres.Where(y => y.ParentGerneId == genre.Id).Select(x => x.Name).ToList();

                    string[] predefinedActionChilds = { "FPS", "TPS" };

                    AddMissingChilds(genre.Id, actionChilds, predefinedActionChilds);

                    break;

                default: break;
            }
        }

        Context.SaveChanges();
    }

    private void AddMissingChilds(Guid parentId, List<string> allChilds, string[] predefinedChilds)
    {
        List<string> missingChilds = predefinedChilds.Except(allChilds).ToList();

        foreach (var missingChild in missingChilds)
        {
            Context.Genres.Add(new Genre(Guid.NewGuid(), missingChild, parentId));
        }
    }
}