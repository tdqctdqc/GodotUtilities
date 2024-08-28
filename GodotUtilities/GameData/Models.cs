using System.Reflection;
using GodotUtilities.Reflection;
using GodotUtilities.Serialization.Depot;

namespace GodotUtilities.GameData;

public class Models
{
    public Dictionary<string, Model> ModelsByName { get; private set; }

    public Models()
    {
        ModelsByName = new Dictionary<string, Model>();
    }

    private void AddModel(Model model)
    {
        ModelsByName.Add(model.Name, model);
    }
    public T GetModel<T>(string name) where T : Model
    {
        return (T)ModelsByName[name];
    }

    public IEnumerable<TModel> GetModels<TModel>() where TModel : Model
    {
        return ModelsByName.Values.OfType<TModel>();
    }
    
    public void ImportWithPredefsAllowDefault<T>(
        Dictionary<string, T> predefs,
        Func<string, T> defaultConstructor,
        DepotImporter importer)
        where T : Model
    {
        ImportWithPredefs(predefs, defaultConstructor, importer);
    }
    public void ImportWithPredefsDisallowDefault<T>(Dictionary<string, T> predefs,
        DepotImporter importer)
        where T : Model
    {
        ImportWithPredefs(predefs, n => throw new Exception(), importer);
    }
    
    private void ImportWithPredefs<T>(Dictionary<string, T> predefs,
        Func<string, T> defaultConstructor,
        DepotImporter importer) 
        where T : Model
    {
        var ms = predefs
            .ToDictionary(t => t.Key,
                t => (object)t);
        var models = importer
            .MakeSheetObjectsModels<T>(ms, defaultConstructor);
        foreach (var model in models)
        {
            AddModel(model);
        }
    }

    public void ImportNoPredefs<T>(Func<string, T> defaultConstructor,
        DepotImporter importer)
        where T : Model, new()
    {
        var models = importer
            .MakeSheetObjectsModels<T>(new Dictionary<string, object>(), 
                defaultConstructor);
        foreach (var model in models)
        {
            AddModel(model);
        }
    }
    
}