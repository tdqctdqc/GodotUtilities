using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using GodotUtilities.GameData;
using GodotUtilities.Reflection;

namespace GodotUtilities.Serialization.Depot;

public class DepotModelSheet
{
    public Dictionary<string, JsonObject> Columns { get; private set; }
    public Dictionary<string, string> ColumnTypeStrings { get; private set; }
    public List<JsonObject> Lines { get; private set; }
    public Type Type { get; set; }
    public string Name { get; private set; }
    public DepotModelSheet(JsonObject sheet, 
        DepotImporter importer)
    {
        Name = JsonSerializer.Deserialize<string>(sheet["name"]);
        Type = importer.ModelTypes[Name];
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

    public void RegisterTypes(DepotImporter importer)
    {
        foreach (var line in Lines)
        {
            var objectGuid = DepotUnpacker.UnpackGuid(line["Model"]);
            var objectName = importer.ModelInstanceNamesByGuid[objectGuid];
            if (importer.ModelInstanceTypes.TryGetValue(objectName, out var oldType))
            {
                if (oldType.IsAssignableFrom(Type))
                {
                    importer.ModelInstanceTypes[objectName] = Type;
                }
            }
            else
            {
                importer.ModelInstanceTypes.Add(objectName, Type);
            }
        }
    }

    public void FillAllLineProperties(DepotImporter importer)
    {
        foreach (var line in Lines)
        {
            FillLineProperties(importer, line);
        }
    }

    private void FillLineProperties(DepotImporter importer,
        JsonObject line)
    {
        var objectGuid = DepotUnpacker.UnpackGuid(line["Model"]);
        var objectName = importer.ModelInstanceNamesByGuid[objectGuid];
        var modelInstance = importer.ModelInstancesByName[objectName];
        var fillColumnProperty = typeof(DepotUnpacker)
            .GetMethod(nameof(DepotUnpacker.FillColumnProperty),
                BindingFlags.Public | BindingFlags.Static);

        foreach (var (propertyName, value) in Columns)
        {
            if (propertyName == "Model") continue;
            Type propertyType;
            try
            {
                propertyType = Type.GetProperty(propertyName).PropertyType;
            }
            catch (Exception e)
            {
                throw new Exception($"couldnt find property {propertyName} in {Type.Name}");
            }
            var columnTypeString = ColumnTypeStrings[propertyName];
            fillColumnProperty.InvokeGeneric(null,
                new[] { propertyType },
                new object[] { importer, modelInstance, 
                    line, propertyName, Type, columnTypeString, Name });
        }
    }

    
}