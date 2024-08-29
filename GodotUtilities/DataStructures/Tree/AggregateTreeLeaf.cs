using Godot;

namespace GodotUtilities.DataStructures.Tree;

public class AggregateTreeLeaf<T>(HashSet<AggregateTreeLeaf<T>> neighbors, HashSet<T> children)
    : IAggregateTreeNode<T>
{
    public HashSet<AggregateTreeLeaf<T>> Neighbors { get; private set; } = neighbors;
    public HashSet<T> Children { get; private set; } = children;

    public static List<AggregateTreeLeaf<T>> MakeRandomish(
        IEnumerable<T> seeds,
        IEnumerable<T> elements,
        int leafSize,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, bool> canShare,
        RandomNumberGenerator random)
    {
        return Make(
            seeds, elements, getNeighbors,
            (seed, open) => FloodFill<T>.FloodFillRandomishToLimit(
                seed, leafSize, getNeighbors,
                n => open.Contains(n) && canShare(seed, n),
                random)
            );
    }
    
    
    public static List<AggregateTreeLeaf<T>> Make(
        IEnumerable<T> seeds,
        IEnumerable<T> elements, 
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, HashSet<T>, HashSet<T>> choose)
    {
        var res = new List<AggregateTreeLeaf<T>>();
        var notTaken = elements.ToHashSet();
        var openSeeds = seeds.ToArray();
        notTaken.ExceptWith(openSeeds);
        var dic = new Dictionary<T, AggregateTreeLeaf<T>>();
        for (var i = 0; i < openSeeds.Length; i++)
        {
            var seed = openSeeds[i];
            var flood = choose(seed, notTaken);
            notTaken.ExceptWith(flood);
            var leaf = new AggregateTreeLeaf<T>(
                new HashSet<AggregateTreeLeaf<T>>(), 
                flood);
            foreach (var t in flood)
            {
                dic.Add(t, leaf);
            }
            res.Add(leaf);
        }
        foreach (var leaf in res)
        {
            var ns = leaf.Children
                .SelectMany(getNeighbors)
                .Distinct()
                .Select(n => dic[n]);
            foreach (var n in ns)
            {
                if (n != leaf)
                {
                    leaf.Neighbors.Add(n);
                }
            }
        }
        return res;
    }
}