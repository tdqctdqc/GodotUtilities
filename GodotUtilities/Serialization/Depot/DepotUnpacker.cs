using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using GodotUtilities.DataStructures;
using GodotUtilities.GameData;
using GodotUtilities.Reflection;

namespace GodotUtilities.Serialization.Depot;

public static class DepotUnpacker
{
    
    
    public static void FillColumnProperty<TProperty>(
        DepotImporter importer,
        Object ob, JsonObject line,
        string propertyName, 
        Type objectType, 
        string columnTypeString,
        string sheetName)
    {
        var columnValue = line[propertyName];
        object value = null;
        var propertyInfo = objectType.GetProperty(propertyName);
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
        else if (columnTypeString == "list")
        {
            value = DepotUnpacker.UnpackList<TProperty>(importer, propertyName, columnValue.AsArray());
        }
        else if (columnTypeString == "lineReference")
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
            GD.Print($"couldnt unpack column type {columnTypeString} " +
                     $"sheet {sheetName} property {propertyName}");
            throw new Exception();
        }

        try
        {
            propertyInfo.SetValue(ob, value);
        }
        catch (Exception e)
        {
            GD.Print($"couldn't set {propertyName} for {objectType.Name}");
            throw;
        }
    }
    
    
    
    public static string UnpackString(JsonNode columnValue)
    {
        return JsonSerializer.Deserialize<string>(columnValue);
    }
    public static bool UnpackBool(JsonNode columnValue)
    {
        return JsonSerializer.Deserialize<bool>(columnValue);
    }
    public static int UnpackInt(JsonNode columnValue)
    {
        return JsonSerializer.Deserialize<int>(columnValue);
    }
    public static float UnpackFloat(JsonNode columnValue)
    {
        return JsonSerializer.Deserialize<float>(columnValue);
    }
    public static Model UnpackLineReference<TProperty>(DepotImporter importer, JsonNode columnValue)
    {
        var name = importer.ModelInstanceNamesByGuid[UnpackGuid(columnValue)];
        return importer.ModelInstancesByName[name];
    }
    public static Guid UnpackGuid(JsonNode columnValue)
    {
        return JsonSerializer.Deserialize<Guid>(columnValue);
    }
    public static TProperty UnpackList<TProperty>(DepotImporter importer,
        string propertyName,
        JsonArray value)
    {
        var propertyType = typeof(TProperty);
        if (propertyType.IsGenericType 
                 && propertyType.GetGenericTypeDefinition() 
                 == typeof(HashSet<>))
        {
            var entryType = propertyType.GetGenericArguments()[0];
            var mi = typeof(DepotUnpacker).GetMethod(nameof(UnpackHashSet),
                BindingFlags.Public | BindingFlags.Static);
            return (TProperty) mi
                .InvokeGeneric(null, 
                    entryType.Yield().ToArray(),
                    new object[] { importer, value, "Value" });
        }
        if (propertyType.IsGenericType 
            && propertyType.GetGenericTypeDefinition() 
            == typeof(Dictionary<,>))
        {
            var keyType = propertyType.GetGenericArguments()[0];
            var valueType = propertyType.GetGenericArguments()[1];
            var mi = typeof(DepotUnpacker).GetMethod(nameof(UnpackDictionary),
                BindingFlags.Public | BindingFlags.Static);
            return (TProperty) mi
                .InvokeGeneric(null, 
                    new []{ keyType, valueType },
                    new object[] { importer, value, "Key", "Value" });
        }
        if (typeof(Array).IsAssignableFrom(propertyType))
        {
            var entryType = propertyType.GetElementType();
            var mi = typeof(DepotUnpacker).GetMethod(nameof(UnpackArray),
                BindingFlags.Public | BindingFlags.Static);
            return (TProperty) mi
                .InvokeGeneric(null, entryType.Yield().ToArray(),
                    new object[] { importer, value, "Value" });
        }
        GD.Print($"no way to import {propertyType.Name} {propertyName}");
        var genericTypeDef = propertyType.GetGenericTypeDefinition();
        GD.Print($"is generic type of {typeof(IdCount<>).Name} {genericTypeDef == typeof(IdCount<>)}");
        throw new Exception();

        return default;
    }
    
    public static TValue[] UnpackArray<TValue>(DepotImporter importer, 
        JsonArray list,
        string columnName)
    {
        return UnpackEnumerable<TValue>(importer, list, columnName)
            .ToArray();
    }
    public static HashSet<TValue> UnpackHashSet<TValue>(
        DepotImporter importer, 
        JsonArray list,
        string columnName)
    {
        return UnpackEnumerable<TValue>(importer, list, columnName)
            .EnumerableToHashSet();
    }

    public static Dictionary<TKey, TValue> UnpackDictionary<TKey, TValue>(
        DepotImporter importer,
        JsonArray list,
        string keyColumnName,
        string valueColumnName)
    {
        var keys = UnpackEnumerable<TKey>(importer, list, keyColumnName)
            .GetEnumerator();
        var values = UnpackEnumerable<TValue>(importer, list, valueColumnName)
            .GetEnumerator();
        var res = new Dictionary<TKey, TValue>();
        while (true)
        {
            var haveKey = keys.MoveNext();
            var haveValue = values.MoveNext();
            if (haveKey != haveValue) throw new Exception();
            if (haveKey == false) break;
            res.Add(keys.Current, values.Current);
        }
        keys.Dispose();
        values.Dispose();
        return res;
    }
    
    public static IEnumerable<TValue> UnpackEnumerable<TValue>(
        DepotImporter importer,
        JsonArray list,
        string columnName)
    {
        Func<JsonNode, TValue> get;
        if (typeof(float).IsAssignableFrom(typeof(TValue)))
        {
            get = a => (TValue)JsonSerializer.Deserialize(a, typeof(float));
        }
        else if (typeof(int).IsAssignableFrom(typeof(TValue)))
        {
            get = a => (TValue)JsonSerializer.Deserialize(a, typeof(int));
        }
        else if (typeof(string).IsAssignableFrom(typeof(TValue)))
        {
            get = a => (TValue)JsonSerializer.Deserialize(a, typeof(string));
        }
        else
        {
            get = a =>
            {
                var name = importer.ModelInstanceNamesByGuid[UnpackGuid(a)];
                return (TValue)((object)importer.ModelInstancesByName[name]);
            };
        }
        for (var i = 0; i < list.Count; i++)
        {
            yield return get(list[i][columnName]);
        }
    }
}