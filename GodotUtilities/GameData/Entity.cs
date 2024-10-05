using GodotUtilities.DataStructures;
using GodotUtilities.Logic;

namespace GodotUtilities.GameData;

public abstract class Entity : IIded
{
    public int Id { get; private set; }

    protected Entity(int id)
    {
        Id = id;
    }

    public void SetId(LogicKey key)
    {
        Id = key.Data.IdDispenser.TakeId();
    }
    public abstract void Made(Data d);
    public abstract void CleanUp(Data d);
}