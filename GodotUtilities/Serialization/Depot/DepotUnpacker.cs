using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using GodotUtilities.DataStructures;
using GodotUtilities.Reflection;

namespace GodotUtilities.Serialization.Depot;

public static class DepotUnpacker
{
    
    
    
    
    
    
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
    public static object UnpackLineReference<TProperty>(DepotImporter importer, JsonNode columnValue)
    {
        var name = importer.ObjectNamesByGuid[UnpackGuid(columnValue)];
        return (TProperty)importer.ObjectsByName[name];
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
                var name = importer.ObjectNamesByGuid[UnpackGuid(a)];
                return (TValue)importer.ObjectsByName[name];
            };
        }
        for (var i = 0; i < list.Count; i++)
        {
            yield return get(list[i][columnName]);
        }
    }
}