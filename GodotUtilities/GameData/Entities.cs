using Godot;
using GodotUtilities.DataStructures;
using GodotUtilities.Serialization;

namespace GodotUtilities.GameData;

public class Entities
{
    public Dictionary<int, Entity> EntitiesById { get; private set; }
    public Entity this[int id] => EntitiesById[id];

    public Entities(Dictionary<int, Entity> entitiesById)
    {
        EntitiesById = entitiesById;
    }


    public void AddEntity(Entity e, Data d)
    {
        if (EntitiesById.ContainsKey(e.Id))
        {
            GD.Print($"trying to overwrite id {e.Id} " +
                     $"{EntitiesById[e.Id].GetType().ToString()} " +
                     $"with {e.GetType().ToString()}");
        }
        EntitiesById.Add(e.Id, e);
        e.Made(d);
    }
    public void RemoveEntity(int eId, Data d)
    {
        var e = EntitiesById[eId];
        e.CleanUp(d);
        EntitiesById.Remove(eId);
    }
    public T Get<T>(int id) where T : Entity
    {
        return (T)EntitiesById[id];
    }
    public IEnumerable<T> GetAll<T>() where T : Entity
    {
        return EntitiesById.Values.OfType<T>().ToHashSet();
    }
    public bool HasEntity(int id)
    {
        return EntitiesById.ContainsKey(id);
    }
}