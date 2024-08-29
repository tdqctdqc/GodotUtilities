using Godot;
using GodotUtilities.DataStructures.Tree;


namespace GodotUtilities.DataStructures.AggregateTree;

public class Branch<T>
    : IAggregate<Branch<T>, Branch<T>>, IAggregate<Branch<T>, T>
{
    public HashSet<Branch<T>> Neighbors { get; private set; } 
    public HashSet<Branch<T>> Children { get; private set; }
    public HashSet<T> Leaves { get; private set; }

    public Branch()
    {
        Neighbors = new HashSet<Branch<T>>();
        Children = new HashSet<Branch<T>>();
        Leaves = new HashSet<T>();
    }

    public T GetFirstLeaf()
    {
        if (Leaves.Count > 0)
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
        //todo dont yield
        foreach (var leaf in Leaves)
        {
            yield return leaf;
        }
        foreach (var branch in Children)
        {
            foreach (var leaf in branch.GetLeaves())
            {
                yield return leaf;
            }
        }
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

}