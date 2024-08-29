using GodotUtilities.DataStructures;

namespace GodotUtilities.GameData;

public readonly struct ModelIdRef<T>(int id = -1) : IIdRef<T>
    where T : Model
{
    public int Id { get; } = id;

    public T Get(Data d)
    {
        var name = d.ModelIdRegister.NamesById[Id];
        return (T)d.Models.ModelsByName[name];
    }
}