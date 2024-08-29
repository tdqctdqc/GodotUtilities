using Godot;

namespace GodotUtilities.DataStructures.Tree;

public class AggregateTreeBranch<T>(HashSet<AggregateTreeBranch<T>> neighbors, HashSet<IAggregateTreeNode<T>> children)
    : IAggregateTreeNode<T>
{
    public HashSet<AggregateTreeBranch<T>> Neighbors { get; private set; } = neighbors;
    public HashSet<IAggregateTreeNode<T>> Children { get; private set; } = children;

    
    public static List<AggregateTreeBranch<T>> AggregateLeavesRandomish(
        IEnumerable<AggregateTreeLeaf<T>> seeds,
        IEnumerable<AggregateTreeLeaf<T>> elements,
        int leafSize,
        Func<AggregateTreeLeaf<T>, AggregateTreeLeaf<T>, bool> canShare,
        RandomNumberGenerator random)
    {
        Func<AggregateTreeLeaf<T>, HashSet<AggregateTreeLeaf<T>>, HashSet<AggregateTreeLeaf<T>>> choose = (seed, open) => FloodFill<AggregateTreeLeaf<T>>.FloodFillRandomishToLimit(
            seed, leafSize, t => t.Neighbors,
            n => open.Contains(n) && canShare(seed, n),
            random);
        return AggregateLeaves(seeds, elements, choose);
    }
    public static List<AggregateTreeBranch<T>> AggregateBranchesRandomish(
        IEnumerable<AggregateTreeBranch<T>> seeds,
        IEnumerable<AggregateTreeBranch<T>> elements,
        int leafSize,
        Func<AggregateTreeBranch<T>, AggregateTreeBranch<T>, bool> canShare,
        RandomNumberGenerator random)
    {
        Func<AggregateTreeBranch<T>, HashSet<AggregateTreeBranch<T>>, HashSet<AggregateTreeBranch<T>>> choose 
            = (seed, open) => FloodFill<AggregateTreeBranch<T>>.FloodFillRandomishToLimit(
            seed, leafSize, t => t.Neighbors,
            n => open.Contains(n) && canShare(seed, n),
            random);
        return AggregateBranches(seeds, elements, choose);
    }
    
    
    
    public static List<AggregateTreeBranch<T>> AggregateLeaves
        (IEnumerable<AggregateTreeLeaf<T>> seeds, 
            IEnumerable<AggregateTreeLeaf<T>> leaves, 
            Func<AggregateTreeLeaf<T>, HashSet<AggregateTreeLeaf<T>>, HashSet<AggregateTreeLeaf<T>>> 
                choose)
    {
        var res = new List<AggregateTreeBranch<T>>();
        var notTaken = leaves.ToHashSet();
        var openSeeds = seeds.ToArray();
        notTaken.ExceptWith(openSeeds);
        var dic = new Dictionary<AggregateTreeLeaf<T>, AggregateTreeBranch<T>>();
        for (var i = 0; i < openSeeds.Length; i++)
        {
            var seed = openSeeds[i];
            var flood = choose(seed, notTaken);
            notTaken.ExceptWith(flood);
            var branch = new AggregateTreeBranch<T>(
                new HashSet<AggregateTreeBranch<T>>(), 
                flood.ToHashSet<IAggregateTreeNode<T>>());
            foreach (var t in flood)
            {
                dic.Add(t, branch);
            }
            res.Add(branch);
        }
        foreach (var branch in res)
        {
            var ns = branch.Children
                .SelectMany(l => ((AggregateTreeLeaf<T>)l).Neighbors)
                .Distinct()
                .Select(n => dic[n]);
            foreach (var n in ns)
            {
                if (n != branch)
                {
                    branch.Neighbors.Add(n);
                }
            }
        }
        return res;
    }
    
    
    public static List<AggregateTreeBranch<T>> AggregateBranches
    (IEnumerable<AggregateTreeBranch<T>> seeds, 
        IEnumerable<AggregateTreeBranch<T>> subBranches, 
        Func<AggregateTreeBranch<T>, HashSet<AggregateTreeBranch<T>>, HashSet<AggregateTreeBranch<T>>> 
            choose)
    {
        var res = new List<AggregateTreeBranch<T>>();
        var notTaken = subBranches.ToHashSet();
        var openSeeds = seeds.ToArray();
        notTaken.ExceptWith(openSeeds);
        var dic = new Dictionary<AggregateTreeBranch<T>, AggregateTreeBranch<T>>();
        for (var i = 0; i < openSeeds.Length; i++)
        {
            var seed = openSeeds[i];
            var flood = choose(seed, notTaken);
            notTaken.ExceptWith(flood);
            var branch = new AggregateTreeBranch<T>(
                new HashSet<AggregateTreeBranch<T>>(), 
                flood.ToHashSet<IAggregateTreeNode<T>>());
            foreach (var t in flood)
            {
                dic.Add(t, branch);
            }
            res.Add(branch);
        }
        foreach (var branch in res)
        {
            var ns = branch.Children
                .SelectMany(l => ((AggregateTreeBranch<T>)l).Neighbors)
                .Distinct()
                .Select(n => dic[n]);
            foreach (var n in ns)
            {
                if (n != branch)
                {
                    branch.Neighbors.Add(n);
                }
            }
        }
        return res;
    }
}