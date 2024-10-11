using Godot;
using GodotUtilities.DataStructures.AggregateTree;
using GodotUtilities.DataStructures.PathFinder;

namespace GodotUtilities.DataStructures.Tree;

public class BranchPathFinder<T>
    where T : class, IIded
{
    private Func<T, T, float> _heuristic;
    private Func<T, bool> _valid;
    private Func<T, T, float> _edgeCost;
    private Func<T, IEnumerable<T>> _getNeighbors;
    public int PathFindDepth { get; set; }
    public HierarchicalCachedPathFinder<Branch<T>, T> PathFinder { get; private set; }
    public Dictionary<Vector2I, List<T>> CachedPaths { get; private set; }
        = new();
    public Dictionary<Vector2I, List<T>> CachedReversePaths { get; private set; }
        = new();
    public Dictionary<Vector2I, float> CachedPathCosts { get; private set; }
        = new();

    public BranchPathFinder(Func<T, T, float> heuristic, 
        Func<T, bool> valid, Func<T, T, float> edgeCost, 
        Func<T, IEnumerable<T>> getNeighbors, int pathFindDepth)
    {
        _heuristic = heuristic;
        _valid = valid;
        _edgeCost = edgeCost;
        _getNeighbors = getNeighbors;
        PathFindDepth = pathFindDepth;
        PathFinder = HierarchicalCachedPathFinder<Branch<T>, T>
            .ConstructAStar(
                (b1, b2) => _heuristic(b1.GetTwigSeed(), b2.GetTwigSeed()),
                h => h.Neighbors.Where(b => _valid(h.GetTwigSeed())),
                _heuristic,
                b => b.GetTwigSeed(),
                _edgeCost,
                h => _getNeighbors(h).Where(_valid)
            );
    }


    public void FindNetworkPaths(IEnumerable<Branch<T>> branches)
    {
        foreach (var branch in branches)
        {
            foreach (var nBranch in branch.Neighbors.Where(b => _valid(b.GetTwigSeed())))
            {
                if (nBranch.Id < branch.Id) continue;
                GetPath(branch, nBranch);
            }
        }
    }

    public List<T> GetPath(Branch<T> from, Branch<T> to)
    {
        var key = from.GetIdEdgeKey(to);
        var reverse = to.Id < from.Id;

        if (CachedPaths.TryGetValue(key, out var cachedPath))
        {
            return reverse ? CachedReversePaths[key] : cachedPath;
        }
        
        var (lo, hi) = reverse ? (to, from) : (from, to);
        
        var loSeed = lo.GetTrunkSeedAtDepth(PathFindDepth);
        var hiSeed = hi.GetTrunkSeedAtDepth(PathFindDepth);
        var path = PathFinder.GetPath(loSeed, hiSeed, 
            out var cost);
        var reversePath = new List<T>(path.Count);
        for (var i = path.Count - 1; i >= 0; i--)
        {
            reversePath.Add(path[i]);
        }
        CachedPaths.Add(key, path);
        CachedReversePaths.Add(key, reversePath);
        CachedPathCosts.Add(key, cost);
        
        return reverse ? reversePath : path;
    }
}