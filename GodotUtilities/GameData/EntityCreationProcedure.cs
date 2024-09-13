using GodotUtilities.Logic;
using GodotUtilities.Server;

namespace GodotUtilities.GameData;

public sealed class EntityCreationProcedure<T> : Procedure
    where T : Entity
{
    public T Entity { get; private set; }
    
    public EntityCreationProcedure(T entity)
    {
        Entity = entity;
    }

    public override void Handle(ProcedureKey key)
    {
        key.Data.Entities.AddEntity(Entity, key.Data);
    }
}


public sealed class EntityCreationProcedure : Procedure
{
    public Entity Entity { get; private set; }
    
    public EntityCreationProcedure(Entity entity)
    {
        Entity = entity;
    }

    public override void Handle(ProcedureKey key)
    {
        key.Data.Entities.AddEntity(Entity, key.Data);
    }
}