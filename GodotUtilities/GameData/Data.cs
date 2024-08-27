using GodotUtilities.DataStructures;
using GodotUtilities.Serialization;

namespace GodotUtilities.GameData;

public class Data
{
    public IdDispenser IdDispenser { get; private set; }
    public Entities Entities { get; private set; }
    public Models Models { get; private set; }
    public Serializer Serializer { get; private set; }

    public static Data CreateForHost()
    {
        var models = new Models(new Dictionary<string, Model>());
        models.ImportFromDepot("\\depot.dpo");
        var serializer = new Serializer();
        var d = new Data(new IdDispenser(0),
            new Entities(new Dictionary<int, Entity>()),
            models, serializer);
        foreach (var (name, model) in models.ModelsByName)
        {
            model.MakeToken(d);
        }
        return d;
    }

    public static Data CreateForRemote()
    {
        var models = new Models(new Dictionary<string, Model>());
        models.ImportFromDepot("\\depot.dpo");
        var serializer = new Serializer();
        var d = new Data(null,
            new Entities(new Dictionary<int, Entity>()),
            models, serializer);
        return d;
    }
    public static Data Load(string path)
    {
        var models = new Models(new Dictionary<string, Model>());
        models.ImportFromDepot("\\depot.dpo");
        var serializer = new Serializer();
        var saveFile = Loader<SaveFile>.Load(path, serializer);
        var d = new Data(saveFile.IdDispenser,
            saveFile.Entities, models, new Serializer());
        return d;
    }

    public Data(IdDispenser idDispenser, Entities entities, Models models, Serializer serializer)
    {
        IdDispenser = idDispenser;
        Entities = entities;
        Models = models;
        Serializer = serializer;
    }
}