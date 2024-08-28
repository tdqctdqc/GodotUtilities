using GodotUtilities.GameData;
using GodotUtilities.Serialization.Depot;

namespace GodotUtilities.GameData;

public abstract class ModelImporter
{
    private string _path;

    protected ModelImporter(string path)
    {
        _path = path;
    }

    public void SetupModels(Models models)
    {
        var importer = new DepotImporter(_path);
        SetupModelsSpecific(models, importer);
    }

    protected abstract void SetupModelsSpecific(Models models, DepotImporter importer);
}