using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using GodotUtilities.Reflection;
using HexGeneral.Game.Components;

namespace GodotUtilities.Serialization.Depot;

public class DepotComponentSheet
{
    public Dictionary<string, JsonObject> Columns { get; private set; }
    public Dictionary<string, string> ColumnTypeStrings { get; private set; }
    public List<JsonObject> Lines { get; private set; }
    public Type Type { get; set; }
    public string Name { get; private set; }
    
    public DepotComponentSheet(JsonObject sheet, 
        DepotImporter importer)
    {
        Name = JsonSerializer.Deserialize<string>(sheet["name"]);
        Type = importer.ComponentTypes[Name];
        Columns = sheet["columns"].AsArray()
            .Select(a => a.AsObject())
            .ToDictionary(
                v => JsonSerializer.Deserialize<string>(v["name"]),
                v => v);
        ColumnTypeStrings = Columns
            .ToDictionary(kvp => kvp.Key,
                kvp => DepotUnpacker.UnpackString(kvp.Value["typeStr"]));
        Lines = new List<JsonObject>();
        foreach (var n in sheet["lines"].AsArray())
        {
            Lines.Add(n.AsObject());
        }
    }
    
    public void MakeAndAddComponents(DepotImporter importer)
    {
        foreach (var line in Lines)
        {
            MakeLineComponent(importer, line);
        }
    }
    
    private T MakeComponentInstance<T>()
        where T : IModelComponent, new()
    {
        return new T();
    }
    
    private void MakeLineComponent(DepotImporter importer,
        JsonObject line)
    {
        var modelGuid = DepotUnpacker.UnpackGuid(line["Model"]);
        var modelName = importer.ModelInstanceNamesByGuid[modelGuid];
        var modelInstance = (IComponentedModel)importer.ModelInstancesByName[modelName];
        var componentInstance = (IModelComponent)GetType()
            .GetMethod(nameof(MakeComponentInstance), 
                BindingFlags.NonPublic | BindingFlags.Instance)
            .InvokeGeneric(this, new[] { Type }, new object[] { });
        
        
        var fillColumnProperty = typeof(DepotUnpacker)
            .GetMethod(nameof(DepotUnpacker.FillColumnProperty),
                BindingFlags.Public | BindingFlags.Static);
        
        foreach (var (propertyName, value) in Columns)
        {
            if (propertyName == "Model") continue;
            var columnTypeString = ColumnTypeStrings[propertyName];
            var propertyType = Type.GetProperty(propertyName).PropertyType;
            fillColumnProperty.InvokeGeneric(null,
                new[] { propertyType },
                new object[] { importer, componentInstance, 
                    line, propertyName, Type, columnTypeString, Name });
        }
        
        modelInstance.Components.Add(componentInstance);
    }

    
}