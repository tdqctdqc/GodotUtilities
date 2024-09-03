using GodotUtilities.GameData;
using GodotUtilities.Serialization.Depot;

namespace GodotUtilities.GameData;

public abstract class ModelImporter
{
    protected string _path;

    protected ModelImporter(string path)
    {
        _path = path;
    }


    public abstract void Import(Models models);
}