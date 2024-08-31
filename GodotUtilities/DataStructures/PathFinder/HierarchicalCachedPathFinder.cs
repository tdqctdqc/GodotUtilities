using Godot;
using GodotUtilities.DataStructures.Tree;

namespace GodotUtilities.DataStructures.PathFinder;

public class HierarchicalCachedPathFinder<TSuper, TSub>
    where TSuper : class, IIded
    where TSub : class, IIded
{
    public CachedPathFinder<TSub> LowLvlPathFinder { get; private set; }
    public CachedPathFinder<TSuper> HiLvlPathFinder { get; private set; }
    public Dictionary<Vector2I, List<TSub>> PathCache { get; private set; }
    public Dictionary<Vector2I, float> PathCostCache { get; private set; }
    public Func<TSuper, TSub> GetSeed { get; private set; }
    public static HierarchicalCachedPathFinder<TSuper, TSub>
        ConstructAStar(Func<TSuper, TSuper, float> getHiLvlEdgeCost,
            Func<TSuper, IEnumerable<TSuper>> getHiLvlNeighbors,
            Func<TSub, TSub, float> heuristic,
            Func<TSuper, TSub> getSeed,
            Func<TSub, TSub, float> getLoLvlEdgeCost,
            Func<TSub, IEnumerable<TSub>> getLoLvlNeighbors)
    {
        
        var hiLvlPathFinder = CachedPathFinder<TSuper>
            .ConstuctAStar(
                getHiLvlEdgeCost,
                getHiLvlNeighbors, 
                (s1, s2) => heuristic(getSeed(s1), getSeed(s2))
        );

        var lowLvlPathFinder = CachedPathFinder<TSub>
            .ConstuctAStar(
                getLoLvlEdgeCost, 
                getLoLvlNeighbors, 
                heuristic);
        
        return new HierarchicalCachedPathFinder<TSuper, TSub>(
            lowLvlPathFinder, 
            hiLvlPathFinder,
            getSeed);
    }

    public HierarchicalCachedPathFinder(
        CachedPathFinder<TSub> lowLvlPathFinder, 
        CachedPathFinder<TSuper> hiLvlPathFinder,
        Func<TSuper, TSub> getSeed)
    {
        LowLvlPathFinder = lowLvlPathFinder;
        HiLvlPathFinder = hiLvlPathFinder;
        PathCache = new Dictionary<Vector2I, List<TSub>>();
        PathCostCache = new Dictionary<Vector2I, float>();
        GetSeed = getSeed;
    }

    public void MakeNetworkPaths(IEnumerable<TSuper> networkNodes,
        Func<TSuper, IEnumerable<TSuper>> getNetworkNeighbors)
    {
        foreach (var node in networkNodes)
        {
            var id = node.Id;
            foreach (var nNode in getNetworkNeighbors(node))
            {
                var nId = nNode.Id;
                if (nId < id) continue;
                GetPath(node, nNode, out var cost);
            }
        }
    }
    
    public List<TSub> GetPath(TSuper start, TSuper end,
        out float cost)
    {
        var key = start.GetIdEdgeKey(end);
        List<TSub> path;
        if (PathCache.TryGetValue(key, out var p))
        {
            (path, cost) = (p, PathCostCache[key]);
        }
        else
        {
            path = FindPath(start, end, out var c);
            cost = c;
            PathCache.Add(key, path);
            PathCostCache.Add(key, cost);
        }
        
        return path;
    }

    private List<TSub> FindPath(TSuper start, TSuper end, 
        out float cost)
    {
        var superPath
            = HiLvlPathFinder.FindPath(start, end, 
                out var superCost);
        var path = new List<TSub>();
        cost = 0f;
        for (var i = 0; i < superPath.Count - 1; i++)
        {
            var from = superPath[i];
            var to = superPath[i + 1];

            var subPath = LowLvlPathFinder
                .FindPath(GetSeed(from), GetSeed(to), 
                    out var subCost);
            path.AddRange(subPath);
            cost += subCost;
        }

        return path;
    }
}

public static class HierarchicalCachedPathFinder
{
    public static HierarchicalCachedPathFinder<TSuper, TSub>
        ConstructAStarAggregate<TSuper, TSub>
            (
                Func<TSuper, TSuper, float> getHiLvlEdgeCost,
                Func<TSub, TSub, float> getLoLvlEdgeCost,
                Func<TSub, TSub, float> heuristic
            )
        where TSuper : class, IIded, IAggregate<TSuper, TSub>, IAggregate<TSuper, TSuper>
        where TSub : class, IIded, INeighbored<TSub>
    {
        return HierarchicalCachedPathFinder<TSuper, TSub>
            .ConstructAStar(
                getHiLvlEdgeCost,
                h => h.Neighbors,
                heuristic,
                h => ((IAggregate<TSuper, TSub>)h).Seed,
                getLoLvlEdgeCost,
                l => l.Neighbors
            );
    }
}