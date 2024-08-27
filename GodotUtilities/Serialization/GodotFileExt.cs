using System.Text;
using Godot;

namespace GodotUtilities.Serialization;

public class GodotFileExt
{

    public static Dictionary<string, Dictionary<string, string>>
        ReadCsvGrid(string path)
    {
        var res = new Dictionary<string, Dictionary<string, string>>();
        
        using (var f = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read))
        {
            string[] fieldNames = null;
            while (f.EofReached() == false)
            {
                var line = f.GetCsvLine("\t");
                if (fieldNames == null)
                {
                    fieldNames = line;
                }
                else
                {
                    var name = line[0];
                    if (name == "") continue;
                    var fields = new Dictionary<string, string>();
                    res.Add(name, fields);
                    for (var i = 1; i < line.Length; i++)
                    {
                        var fieldName = fieldNames[i];
                        if (fieldName == "") continue;
                        var value = line[i];
                        fields.Add(fieldName, value);
                    }
                }
            }
        }

        return res;
    }
    public static void SaveFile<T>(T t, string path, string name,
        string ext, Serializer serializer)
    {
        var bytes = serializer.Serialize(t);
        var dir = DirAccess.Open(path);
        var fileAccess = Godot.FileAccess.Open(name+ext, Godot.FileAccess.ModeFlags.Write);
        fileAccess.StoreBuffer(bytes);
        fileAccess.Close();
    }
    public static T LoadFileAs<T>(string path, string name, 
        string ext, Serializer serializer)
    {
        var filePaths = GetAllFilePathsOfType(path, ext);
        foreach (var filePath in filePaths)
        {
            var fileName = GetFileName(filePath);
            if (fileName == name)
            {
                var fullPath = path + name + ext;
                var f = Godot.FileAccess.Open(fullPath, Godot.FileAccess.ModeFlags.Read);
                var bytes = f.GetBuffer((long)f.GetLength());
                f.Close();
                return serializer.Deserialize<T>(bytes);
            }
        }
        throw new Exception($"couldn't find {name} in {path}");
    }
    public static List<string> GetAllFilePathsOfType(string path, string type)
    {
        var filePaths = new List<string>();
        var dir = DirAccess.Open(path);
        if (dir == null) return filePaths;
        dir.ListDirBegin();
        var filename = dir.GetNext();
        while(filename != "")
        {
            if (dir.CurrentIsDir() && filename.StartsWith(".") == false)
            {
                filePaths.AddRange(GetAllFilePathsOfType(path.PathJoin(filename), type));
            }
            else if(filename.EndsWith(type))
            {
                filePaths.Add(path.PathJoin(filename));
            }

            filename = dir.GetNext();
        }

        return filePaths;
    }
    
    
    public static List<string> GetAllFilePathsOfTypes(string path, List<string> types)
    {
        var filePaths = new List<string>();
        var dir = DirAccess.Open(path);
        if (dir == null) return filePaths;
        dir.ListDirBegin();
        var filename = dir.GetNext();
        while(filename != "")
        {
            if (dir.CurrentIsDir() && filename.StartsWith(".") == false)
            {
                filePaths.AddRange(GetAllFilePathsOfTypes(path.PathJoin(filename), types));
            }
            else if(types.Any(t => filename.EndsWith(t)))
            {
                filePaths.Add(path.PathJoin(filename));
            }

            filename = dir.GetNext();
        }

        return filePaths;
    }

    public static string ReadFileAsString(string path)
    {
        var f = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
        var sb = new StringBuilder();
        while (f.EofReached() == false)
        {
            sb.Append(f.GetLine());
        }
        return sb.ToString();
    }

    public static string GetFileName(string path)
    {
        var lastSlash = path.LastIndexOf("/");
        var period = path.LastIndexOf(".");
        var length = period - lastSlash - 1;
        return path.Substring(lastSlash + 1, length);
    }
}