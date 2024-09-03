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
    
}