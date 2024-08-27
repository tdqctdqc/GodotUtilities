using System.Reflection;
using GodotUtilities.DataStructures;
using GodotUtilities.Reflection;

namespace GodotUtilities.GameData;

public abstract class Model
{
    public string Name { get; private set; }

    protected Model(string name)
    {
    }

    public void MakeToken(Data d)
    {
        var t = GetType();
        var m = t.GetMethod(nameof(AddToken),
            BindingFlags.Instance | BindingFlags.NonPublic);
        m.InvokeGeneric(this, new Type[] { t },
            new object[] { d });
    }

    private void AddToken<T>(Data d) where T : Model
    {
        var token = new ModelToken<T>(d.IdDispenser.TakeId(), Name);
        d.Entities.AddEntity(token, d);
    }
}