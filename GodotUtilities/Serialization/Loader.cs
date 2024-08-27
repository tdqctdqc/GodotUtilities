
using System;
using System.Collections.Generic;
using GodotUtilities.Serialization;
using Godot;
using FileAccess = Godot.FileAccess;

public class Loader<T>
{
    public static T Load(string path, Serializer serializer)
    {
        var fileAccess = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var bytes = fileAccess.GetBuffer((long)fileAccess.GetLength());
        fileAccess.Close();
        return serializer.Deserialize<T>(bytes);
    }
    
    public static List<T> LoadFromJson(string folder,
        string fileEnding, Func<string, T> construct)
    {
        var res = new List<T>();
        var paths = GodotFileExt.GetAllFilePathsOfTypes(folder, 
            new List<string>{ fileEnding });
        paths.ForEach(path =>
        {
            var json = GodotFileExt.ReadFileAsString(path);
            var t = construct(json);
            res.Add(t);
        });
        return res;
    }
}