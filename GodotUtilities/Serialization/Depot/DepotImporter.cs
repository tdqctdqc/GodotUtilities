using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using GodotUtilities.GameData;
using GodotUtilities.Reflection;

namespace GodotUtilities.Serialization.Depot;

public class DepotImporter
{
    public Dictionary<string, object> ObjectsByName { get; private set; }
    public Dictionary<Guid, string> ObjectNamesByGuid { get; private set; }
    public Dictionary<string, Type> ModelInstanceTypes { get; private set; }
    public Dictionary<string, DepotModelSheet> ModelSheets { get; private set; }
    public Dictionary<string, Type> ModelTypes { get; private set; }
    
    public DepotImporter(string path, IEnumerable<Type> modelTypes,
        Models models)
    {
        path = "res://" + path;
        var json = GodotFileExt.ReadFileAsString(path);
        ModelTypes = modelTypes.Distinct()
            .ToDictionary(t => t.Name, t => t);
        ObjectsByName = new Dictionary<string, object>();
        ObjectNamesByGuid = new Dictionary<Guid, string>();
        ModelSheets = new Dictionary<string, DepotModelSheet>();
        ModelInstanceTypes = new Dictionary<string, Type>();
        
        MakeSheets(json);
        RegisterObjectTypes();
        MakeObjects();
        foreach (var (sheetName, sheet) in ModelSheets)
        {
            sheet.FillAllLineProperties(this);
        }

        var modelNameProperty = typeof(Model)
            .GetProperty(nameof(Model.Name));
        foreach (var (name, ob) in ObjectsByName)
        {
            var modelName = name.Split("_").Last();
            modelNameProperty.SetValue(ob, modelName);
            models.ModelsByName.Add(modelName, (Model)ob);
        }
    }
    
    private void MakeSheets(string json)
    {
        var top = JsonSerializer
            .Deserialize<JsonObject>(json);
        var sheets = top["sheets"].AsArray();
        
        
        foreach (var n in sheets)
        {
            var name = JsonSerializer.Deserialize<string>(n["name"]);
            var sheetJsonObject = n.AsObject();

            if (name == "ModelNames")
            {
                AddModelNames(sheetJsonObject);
                continue;
            }
            
            if (sheetJsonObject.ContainsKey("hidden")
                && JsonSerializer.Deserialize<bool>(sheetJsonObject["hidden"]))
            {
                continue;
            }
            
            var sheet = new DepotModelSheet(sheetJsonObject, this);
            ModelSheets.Add(sheet.Name, sheet);
            
            
        }
    }

    private void AddModelNames(JsonObject jsonObject)
    {
        var lines = jsonObject["lines"].AsArray();
        foreach (var line in lines)
        {
            var name = DepotUnpacker.UnpackString(line["Name"]);
            var guid = DepotUnpacker.UnpackGuid(line["guid"]);
            ObjectNamesByGuid.Add(guid, name);
        }
    }

    private void RegisterObjectTypes()
    {
        foreach (var (name, sheet) in ModelSheets)
        {
            sheet.RegisterTypes(this);
        }
    }
    
    
    private void MakeObjects()
    {
        foreach (var (name, type) in ModelInstanceTypes)
        {
            var mi = GetType().GetMethod(nameof(MakeObject),
                BindingFlags.NonPublic | BindingFlags.Instance);
            mi.InvokeGeneric(this, new[] { type },
                    new object[] { name });
        }
    }

    private void MakeObject<T>(string name)
        where T : new()
    {
        var ob = new T();
        ObjectsByName.Add(name, ob);
    }
}