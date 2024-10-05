using GodotUtilities.GameData;
using GodotUtilities.Logic;
using GodotUtilities.Server;

namespace GodotUtilities.GameClient;

public class EntityCreationCommand<T>(T entity) : Command
    where T : Entity
{
    public T Entity { get; private set; } = entity;

    public override void Handle(LogicKey key)
    {
        Entity.SetId(key);
        var proc = new EntityCreationProcedure<T>(Entity);
        key.SendMessage(proc);
    }
}