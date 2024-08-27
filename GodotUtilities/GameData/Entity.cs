using GodotUtilities.DataStructures;
namespace GodotUtilities.GameData;

public abstract class Entity : IIded
{
    public int Id { get; }

    protected Entity(int id)
    {
        Id = id;
    }

    public abstract void Made(Data d);
    public abstract void CleanUp(Data d);
}