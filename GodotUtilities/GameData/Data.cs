using GodotUtilities.DataStructures;
using GodotUtilities.Serialization;
using GodotUtilities.Serialization.Depot;

namespace GodotUtilities.GameData;

public class Data
{
    public IdDispenser IdDispenser { get; private set; }
    public Entities Entities { get; private set; }
    public Models Models { get; private set; }
    public Serializer Serializer { get; private set; }

    public static void SetupForHost(Data d, ModelImporter modelImporter)
    {
        modelImporter.SetupModels(d.Models);
        foreach (var (name, model) in d.Models.ModelsByName)
        {
            model.MakeToken(d);
        }
    }
    public static void SetupForRemote(Data d, ModelImporter modelImporter)
    {
        modelImporter.SetupModels(d.Models);
    }
    public static void SetupForLoad(Data d, string path, ModelImporter modelImporter)
    {
        var models = new Models();
        var serializer = new Serializer();
        var saveFile = Loader<SaveFile>.Load(path, serializer);
        d.IdDispenser = saveFile.IdDispenser;
        d.Entities = saveFile.Entities;
        modelImporter.SetupModels(models);
    }

    public Data(IdDispenser idDispenser, Entities entities, Models models, Serializer serializer)
    {
        IdDispenser = idDispenser;
        Entities = entities;
        Models = models;
        Serializer = serializer;
    }
}