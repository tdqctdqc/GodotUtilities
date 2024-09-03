
using System.Collections.Generic;
using Godot;
using GodotUtilities.Serialization;

public class TextureManager
{
    public static Dictionary<string, Texture2D> Textures { get; private set; }
    public static void Setup(string path, List<string> fileEndings)
    {
        Textures = new Dictionary<string, Texture2D>();
        var scenePaths = GodotFileExt.GetAllFilePathsOfTypes(path, 
            fileEndings);
        scenePaths.ForEach(path =>
        {
            var text = (Texture2D) GD.Load(path);
            var textureName = GodotFileExt.GetFileName(path.ToLower());
            Textures.Add(textureName, text);
        });
    }
}
