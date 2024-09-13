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
    public JsonObject Sheet { get; private set; }
    public List<JsonObject> Lines { get; private set; }
    public Type Type { get; set; }
    public string Name { get; private set; }
    public DepotModelSheet(JsonObject sheet, 
        DepotImporter importer)
    {
        Name = JsonSerializer.Deserialize<string>(sheet["name"]);
        Type = importer.Types[Name];
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
            var objectName = importer.ObjectNamesByGuid[objectGuid];
            if (importer.ObjectTypes.TryGetValue(objectName, out var oldType))
            {
                if (oldType.IsAssignableFrom(Type))
                {
                    importer.ObjectTypes[objectName] = Type;
                }
            }
            else
            {
                importer.ObjectTypes.Add(objectName, Type);
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
        var objectName = importer.ObjectNamesByGuid[objectGuid];
        var ob = importer.ObjectsByName[objectName];
        var fillColumnProperty = this.GetType()
            .GetMethod(nameof(FillColumnProperty),
                BindingFlags.NonPublic | BindingFlags.Instance);
        
        foreach (var (propertyName, value) in Columns)
        {
            if (propertyName == "Model") continue;

            var propertyType = Type.GetProperty(propertyName).PropertyType;
            fillColumnProperty.InvokeGeneric(this,
                new[] { propertyType },
                new[] { importer, ob, line, propertyName });
            
            

            
        }
    }

    private void FillColumnProperty<TProperty>(DepotImporter importer,
        Object ob, JsonObject line,
        string propertyName)
    {
        var columnValue = line[propertyName];
        var columnType = ColumnTypeStrings[propertyName];
        object value = null;
        var propertyInfo = Type.GetProperty(propertyName);
        var propertyType = typeof(TProperty);
        if (propertyType == typeof(float))
        {
            value = DepotUnpacker.UnpackFloat(columnValue);
        }
        else if (propertyType == typeof(int))
        {
            value = DepotUnpacker.UnpackInt(columnValue);
        }
        else if (propertyType == typeof(string))
        {
            value = DepotUnpacker.UnpackString(columnValue);
        }
        else if (columnType == "list")
        {
            value = DepotUnpacker.UnpackList<TProperty>(importer, propertyName, columnValue.AsArray());
        }
        else if (columnType == "lineReference")
        {
            value = DepotUnpacker.UnpackLineReference<TProperty>(importer, columnValue);
        }
        else if (propertyType == typeof(bool))
        {
            value = DepotUnpacker.UnpackBool(columnValue);
        }
        else if (propertyType == typeof(Color))
        {
            var colorString = DepotUnpacker.UnpackString(columnValue);
            value = new Color(colorString);
        }
        else
        {
            GD.Print($"couldnt unpack column type {columnType} " +
                     $"sheet {Name} property {propertyName}");
            throw new Exception();
        }

        try
        {
            propertyInfo.SetValue(ob, value);
        }
        catch (Exception e)
        {
            GD.Print($"couldn't set {propertyName} for {Type.Name}");
            throw;
        }
    }
    
    
}