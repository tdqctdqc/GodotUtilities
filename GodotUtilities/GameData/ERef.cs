using Godot;
using System;
using System.Collections.Generic;
using GodotUtilities.DataStructures;
namespace GodotUtilities.GameData;
using MessagePack;

public readonly struct ERef<TEntity> : IIdRef<TEntity>
    where TEntity : Entity
{
    public int Id { get; }

    public static bool operator ==(ERef<TEntity> r, TEntity e)
        => r.Id == e.Id;
    public static bool operator !=(ERef<TEntity> r, TEntity e) 
        => !(r == e);
    
    public static bool operator ==(ERef<TEntity> r, ERef<TEntity> e) 
        => r.Id == e.Id;
    public static bool operator !=(ERef<TEntity> r, ERef<TEntity> e) 
        => r.Id != e.Id;

    public ERef()
    {
        Id = -1;
    }
    public ERef(TEntity entity)
    {
        Id = entity.Id;
    }

    public static ERef<TEntity> GetEmpty()
    {
        return new ERef<TEntity>(-1);
    }
    [SerializationConstructor] public ERef(int id)
    {
        Id = id;
    }

    public TEntity Get(Data data)
    {
        if (Id == -1) return null;
        return data.Entities.Get<TEntity>(Id);
    }
}
