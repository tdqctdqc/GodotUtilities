using System.Reflection;
using GodotUtilities.Reflection;
using GodotUtilities.Serialization.Depot;

namespace GodotUtilities.GameData;

public class Models
{
    public Dictionary<string, Model> ModelsByName { get; private set; }

    public Models(Dictionary<string, Model> modelsByName)
    {
        ModelsByName = modelsByName;
    }

    public void ImportFromDepot(string filePath)
    {
        var importer = new DepotImporter(filePath);
        //for each model type do import
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
    
    private void ImportWithPredefsAllowDefault<T>(
        List<T> predefs,
        DepotImporter importer)
        where T : Model, new()
    {
        ImportWithPredefs(predefs, () => new(), importer);
    }
    private void ImportWithPredefsDisallowDefault<T>(List<T> predefs,
        DepotImporter importer)
        where T : Model
    {
        ImportWithPredefs(predefs, () => throw new Exception(), importer);
    }
    
    private void ImportWithPredefs<T>(List<T> predefs,
        Func<T> defaultConstructor,
        DepotImporter importer) 
        where T : Model
    {
        var ms = predefs.GetPropertiesOfTypeByName<T>()
            .ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);
        var models = importer.MakeSheetObjectsModels<T>(ms, defaultConstructor);
        foreach (var model in models)
        {
            AddModel(model);
        }
    }

    private void ImportNoPredefs<T>(DepotImporter importer)
        where T : Model, new()
    {
        var models = importer
            .MakeSheetObjectsModels<T>(new Dictionary<string, object>(), 
                () => new T());
        foreach (var model in models)
        {
            AddModel(model);
        }
    }
    
}