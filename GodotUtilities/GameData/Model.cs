using System.Reflection;
using GodotUtilities.DataStructures;
using GodotUtilities.Reflection;

namespace GodotUtilities.GameData;

public abstract class Model
{
    public string Name { get; protected set; }

    protected Model(string name)
    {
        Name = name;
    }

    public void MakeToken(Data d)
    {
        var t = GetType();
        var m = typeof(Model).GetMethod(nameof(AddToken),
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (m is null) throw new Exception();
        m.InvokeGeneric(this, new Type[] { t },
            new object[] { d });
    }

    private void AddToken<T>(Data d) where T : Model
    {
        var token = new ModelToken<T>(d.IdDispenser.TakeId(), Name);
        d.Entities.AddEntity(token, d);
    }

    public void SetName(string name)
    {
        Name = name;
    }
}