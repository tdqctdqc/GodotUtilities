using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using GodotUtilities.GameData;

namespace GodotUtilities.Serialization.Depot;

public class DepotSheet
{
    public Dictionary<string, JsonObject> Columns { get; private set; }
    public JsonObject Sheet { get; private set; }
    public Dictionary<string, JsonObject> LinesByName { get; private set; }
    public Dictionary<string, Guid> LineGuids { get; private set; }
    public Type Type { get; set; }
    public string Name { get; private set; }
    public DepotSheet(JsonObject sheet, DepotImporter importer)
    {
        Name = JsonSerializer.Deserialize<string>(sheet["name"]);
        var sheetGuid = JsonSerializer.Deserialize<Guid>(sheet["guid"]);
        importer.Sheets.Add(Name, this);
        importer.SheetsByGuid.Add(sheetGuid, this);
        Columns = sheet["columns"].AsArray()
            .Select(a => a.AsObject())
            .ToDictionary(
                v => JsonSerializer.Deserialize<string>(v["name"]),
                v => v);
        LinesByName = new Dictionary<string, JsonObject>();
        LineGuids = new Dictionary<string, Guid>();
        foreach (var n in sheet["lines"].AsArray())
        {
            var line = n.AsObject();
            var lineName = JsonSerializer.Deserialize<string>(line["Name"]);
            var lineGuid = JsonSerializer.Deserialize<Guid>(line["guid"]);
            importer.LinesByGuid.Add(lineGuid, line);
            importer.LinesByName.Add(lineName, line);
            LinesByName.Add(lineName, line);
            LineGuids.Add(lineName, lineGuid);
        }
    }

    public void MakeObjectsDefault<T>(Func<T> get, DepotImporter importer)
    {
        foreach (var (lineName, line) in LinesByName)
        {
            var t = get();
            var lineGuid = LineGuids[lineName];
            importer.LineObjects.Add(lineGuid, t);
            importer.LineObjectsByName.Add(lineName, t);
        }
    }

    public IEnumerable<T> MakeObjectsModels<T>(
        IReadOnlyDictionary<string, object> models, 
        Func<string, T> defaultConstructor,
        DepotImporter importer)
        where T : Model
    {
        foreach (var (name, value) in models)
        {
            if (value is T t == false)
            {
                GD.Print($"{name} is not {typeof(T).Name}");
                throw new Exception();
            }
            var guid = LineGuids[name];
            importer.LineObjects[guid] = value;
            importer.LineObjectsByName[name] = value;
            yield return t;
        }

        foreach (var (name, line) in LinesByName)
        {
            if (models.ContainsKey(name)) continue;
            var t = defaultConstructor(name);
            var guid = LineGuids[name];
            importer.LineObjects[guid] = t;
            importer.LineObjectsByName[name] = t;
            yield return t;
        }
    }
}