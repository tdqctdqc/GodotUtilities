using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotUtilities.GameData;
using MessagePack;

namespace GodotUtilities.DataStructures;
public class IdCount<T> : Count<int> 
    
    where T : IIded
{
    public float Get(T t) => Get(t.Id);

    public static IdCount<T> Sum(params IdCount<T>[] toSum)
    {
        var res = IdCount<T>.Construct();
        foreach (var idCount in toSum)
        {
            res.Add(idCount);
        }

        return res;
    }
    public static IdCount<T> Construct()
    {
        return new IdCount<T>(new Dictionary<int, float>(), false);
    }
    public static IdCount<T> ConstructNegative()
    {
        return new IdCount<T>(
            new Dictionary<int, float>(), 
            true);
    }
    public static IdCount<T> Construct(IdCount<T> toCopy)
    {
        return new IdCount<T>(new Dictionary<int, float>(toCopy.Contents), false);
    }
    public static IdCount<T> Construct<TCopy>(IdCount<TCopy> toCopy)
        where TCopy : T
    {
        return new IdCount<T>(new Dictionary<int, float>(toCopy.Contents), false);
    }
    public static IdCount<T> ConstructNegative(IdCount<T> toCopy)
    {
        return new IdCount<T>(
            new Dictionary<int, float>(toCopy.Contents),
            true);
    }
    public static IdCount<TSuper> Construct<TSuper, TSub>(IdCount<TSub> toCopy)
        where TSuper : IIded where TSub : IIded
    {
        return new IdCount<TSuper>(new Dictionary<int, float>(toCopy.Contents), false);
    }

    public static IdCount<T> Construct(params (T, float)[] entries)
    {
        return new IdCount<T>(
            entries.ToDictionary(e => e.Item1.Id, e => e.Item2),
            false);
    }
    public static IdCount<T> Construct(Dictionary<T, float> toCopy)
    {
        return new IdCount<T>(toCopy.ToDictionary(kvp => kvp.Key.Id, kvp => kvp.Value),
            false);
    }
    public IdCount(Dictionary<int, float> contents, bool canBeNegative) 
        : base(contents, canBeNegative)
    {
    }
    
    
    public IEnumerable<KeyValuePair<T, float>> GetEnumerable(Data d, Func<int, Data, T> get)
    {
        return Contents.Select(kvp => 
            new KeyValuePair<T, float>(get(kvp.Key, d), kvp.Value));
    }
    public void Add<T2>(IdCount<T2> count)
        where T2 : T
    {
        foreach (var (key, value) in count.Contents)
        {
            Add(key, value);
        }
    }
    public void Add<T2>(Dictionary<T2, float> count)
        where T2 : T
    {
        foreach (var (key, value) in count)
        {
            Add(key, value);
        }
    }
    public void Add(T model, float amount)
    {
        if (amount == 0) return;
        Add(model.Id, amount);
    }

    public void Set(T model, float amount)
    {
        Contents[model.Id] = amount;
    }
    public void Set(int id, float amount)
    {
        if (amount == 0f)
        {
            Contents.Remove(id);
        }
        else
        {
            Contents[id] = amount;
        }
    }
    public void Remove(T model, float amount)
    {
        if (amount == 0) return;
        try
        {
            Remove(model.Id, amount);
        }
        catch (Exception e)
        {
            GD.Print("problem removing " + model.GetType().Name);
            GD.Print("trying to remove " + amount + " only " + Get(model));
            throw;
        }
    }
    public static IdCount<T> Union<T>(params IdCount<T>[] counts)
        where T : IIded
    {
        var res = IdCount<T>.Construct();
        foreach (var count in counts)
        {
            foreach (var kvp in count.Contents)
            {
                res.Add(kvp.Key, kvp.Value);
            }
        }
        return res;
    }
    public void Subtract<T>(IdCount<T> take) where T : IIded
    {
        foreach (var kvp in take.Contents)
        {
            Remove(kvp.Key, kvp.Value);
        }
    }
}
