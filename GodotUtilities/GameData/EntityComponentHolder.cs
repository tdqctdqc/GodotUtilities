using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GodotUtilities.Logic;
using MessagePack;

namespace GodotUtilities.GameData;

public class EntityComponentHolder
{
    public List<IEntityComponent> EntityComponents { get; private set; }
    public List<ModelIdRef<Model>> Models { get; private set; } 
    public void Initialize(
        IComponentedEntity entity,
        List<IEntityComponent> components,
        Data data,
        params IComponentedModel[] models)
    {
        foreach (var c in components)
        {
            Add(c, data);
        }
        foreach (var cm in models)
        {
            AddModel(entity, cm, data);
        }
    }

    public static EntityComponentHolder Construct()
    {
        return new EntityComponentHolder(new List<IEntityComponent>(),
            new List<ModelIdRef<Model>>());
    }

    [SerializationConstructor] private EntityComponentHolder(
        List<IEntityComponent> entityComponents, 
        List<ModelIdRef<Model>> models)
    {
        EntityComponents = entityComponents;
        Models = models;
    }
    
    public void Add(IEntityComponent c, Data data)
    {
        EntityComponents.Add(c);
        c.Added(this, data);
    }

    private void AddModel(IComponentedEntity entity, 
        IComponentedModel model, Data data)
    {
        Models.Add(((Model)model).MakeIdRef(data));
        foreach (var c in model.Components.OfType<IInheritedModelComponent>())
        {
            c.InheritTo(entity, data);
        }
    }
    public void Remove(IEntityComponent c, Data data)
    {
        EntityComponents.Remove(c);
        c.Removed(this, data);
    }

    private IEnumerable<IComponentedModel> GetModels(Data data)
    {
        return Models.Select(m => (IComponentedModel)m.Get(data));
    }
    public void TurnTick(ProcedureKey key)
    {
        foreach (var component in EntityComponents)
        {
            component.TurnTick(key);
        }
    }
    public T Get<T>(Data data) where T : IEntityComponent
    {
        return All(data).OfType<T>().FirstOrDefault();
    }


    public IEnumerable<T> OfType<T>(Data data) where T : IEntityComponent
    {
        return EntityComponents.OfType<T>()
            .Concat(GetModels(data).SelectMany(m => m.Components.OfType<T>()));
    }

    public IEnumerable<IComponent> All(Data data)
    {
        return EntityComponents
            .AsEnumerable<IComponent>()
            .Concat(GetModels(data)
                .SelectMany(m => m.Components.Components));
    }
}