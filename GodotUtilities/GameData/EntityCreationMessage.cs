using GodotUtilities.Server;

namespace GodotUtilities.GameData;

public sealed class EntityCreationMessage<T> : Message
    where T : Entity
{
    public T Entity { get; private set; }
    
    public EntityCreationMessage(T entity)
    {
        Entity = entity;
    }

    public override void Handle()
    {
        throw new NotImplementedException();
    }
}


public sealed class EntityCreationMessage : Message
{
    public Entity Entity { get; private set; }
    
    public EntityCreationMessage(Entity entity)
    {
        Entity = entity;
    }

    public override void Handle()
    {
        throw new NotImplementedException();
    }
}