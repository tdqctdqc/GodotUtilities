using GodotUtilities.DataStructures;

namespace GodotUtilities.GameData;

public class ModelIdRef<T> : IIdRef<T> 
    where T : Model
{
    public int Id { get; }

    public ModelIdRef(int id)
    {
        Id = id;
    }

    public T Get(Data d)
    {
        return d.Entities.Get<ModelToken<T>>(Id).Get(d);
    }
}