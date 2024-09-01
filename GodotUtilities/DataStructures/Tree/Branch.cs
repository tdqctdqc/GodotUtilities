using System.Runtime.CompilerServices;
using Godot;
using GodotUtilities.DataStructures.Tree;


namespace GodotUtilities.DataStructures.AggregateTree;

public class Branch<T>
    : IAggregate<Branch<T>, Branch<T>>, IAggregate<Branch<T>, T>,
        IIded
    where T : IIded
{
    public HashSet<Branch<T>> Neighbors { get; private set; } 
    public HashSet<Branch<T>> Children { get; private set; }
    public HashSet<T> Leaves { get; private set; }
    public Branch<T> TrunkSeed { get; private set; }
    public T TwigSeed { get; private set; }
    public int Id { get; private set; }
    public static Branch<T> ConstructTwig(T seed)
    {
        var b = new Branch<T>(new HashSet<Branch<T>>(),
            null, new HashSet<T>(), seed.Id);
        b.SetSeed(seed);
        return b;
    }

    
    
    public static Branch<T> ConstructTrunk(Branch<T> seed)
    {
        var b = new Branch<T>(new HashSet<Branch<T>>(),
            new HashSet<Branch<T>>(), null, seed.Id);
        b.SetSeed(seed);
        return b;
    }
    private Branch(HashSet<Branch<T>> neighbors, 
        HashSet<Branch<T>> children, HashSet<T> leaves,
        int id)
    {
        Id = id;
        Neighbors = neighbors;
        Children = children;
        Leaves = leaves;
    }

    public T GetFirstLeaf()
    {
        if (Leaves?.Count > 0)
        {
            return Leaves.First();
        }

        foreach (var child in Children)
        {
            var t = child.GetFirstLeaf();
            if (t is not null && t.Equals(default) == false)
            {
                return t;
            }
        }

        return default;
    }
    public IEnumerable<T> GetLeaves()
    {
        if (Leaves is not null)
        {
            return Leaves;
        }

        return Children.SelectMany(c => c.GetLeaves());
    }

    public IEnumerable<Branch<T>> GetTwigs()
    {
        if (Children is not null)
        {
            return Children.SelectMany(c => c.GetTwigs());
        }

        return this.Yield();
    }
    public void AddNeighbor(Branch<T> t)
    {
        Neighbors.Add(t);
    }

    public void RemoveNeighbor(Branch<T> t)
    {
        Neighbors.Remove(t);
    }

    public void AddChild(T t)
    {
        Leaves.Add(t);
    }

    public void RemoveChild(T t)
    {
        Leaves.Remove(t);
    }

    public void AddChild(Branch<T> t)
    {
        Children.Add(t);
    }

    public void RemoveChild(Branch<T> t)
    {
        Children.Remove(t);
    }

    IEnumerable<Branch<T>> INeighbored<Branch<T>>.Neighbors => Neighbors;

    IEnumerable<Branch<T>> IChildrened<Branch<T>>.Children => Children;

    IEnumerable<T> IChildrened<T>.Children => Leaves;

    Branch<T> IAggregate<Branch<T>, Branch<T>>.Seed => TrunkSeed;

    public Branch<T> GetTrunkSeedAtDepth(int depth)
    {
        if (depth == 0) return this;
        return TrunkSeed.GetTrunkSeedAtDepth(depth - 1);
    }

    public IEnumerable<Branch<T>> GetBranchesAtDepth(int depth)
    {
        if (depth == 0) return this.Yield();
        return Children.SelectMany(c => c.GetBranchesAtDepth(depth - 1));
    }
    
    public void SetSeed(T seed)
    {
        TwigSeed = seed;
    }

    public void SetSeed(Branch<T> seed)
    {
        TrunkSeed = seed;
    }

    T IAggregate<Branch<T>, T>.Seed => GetTwigSeed();
    
    public T GetTwigSeed()
    {
        if (TwigSeed is not null) return TwigSeed;
        return TrunkSeed.GetTwigSeed();
    }
}