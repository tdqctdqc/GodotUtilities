using Godot;

namespace GodotUtilities.GameData;

public class ModelIdRegister(int id, Dictionary<int, string> namesById, Dictionary<string, int> idsByName) : Entity(id)
{
    public Dictionary<int, string> NamesById { get; private set; } = namesById;
    public Dictionary<string, int> IdsByName { get; private set; } = idsByName;

    public void Register(Model m, Data d)
    {
        var id = d.IdDispenser.TakeId();
        NamesById.Add(id, m.Name);
        IdsByName.Add(m.Name, id);
    }
    public override void Made(Data d)
    {
        d.SetEntitySingleton<ModelIdRegister>();
    }

    public override void CleanUp(Data d)
    {
        throw new Exception();
    }

    
}