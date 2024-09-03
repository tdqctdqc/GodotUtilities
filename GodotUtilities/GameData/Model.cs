using System.Reflection;
using GodotUtilities.DataStructures;
using GodotUtilities.Reflection;

namespace GodotUtilities.GameData;

public abstract class Model
{
    public string Name { get; protected set; }

    protected Model()
    {
    }



    public void SetName(string name)
    {
        Name = name;
    }
    
}