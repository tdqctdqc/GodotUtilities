using Godot;

namespace GodotUtilities.DataStructures.PathFinder;

public class CachedPathFinder<T>
    where T : class, IIded
{
    public Dictionary<Vector2I, float> EdgeCostCache { get; private set; }
        = new ();
    public Dictionary<Vector2I, List<T>> PathCache { get; private set; }
        = new();
    public Dictionary<Vector2I, List<T>> ReversePathCache { get; private set; }
        = new();
    public Dictionary<Vector2I, float> PathCostCache { get; private set; }
        = new();

    private Func<T, T, float> _getEdgeCost;
    private Func<T, T, (List<T>, float)> _findPath;

    public static CachedPathFinder<T> ConstuctAStar(Func<T, T, float> getCost, 
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, float> heuristicFunc)
    {
        return new CachedPathFinder<T>(
            getCost, getNeighbors, heuristicFunc);
    }
    private CachedPathFinder(Func<T, T, float> getEdgeCost, 
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, float> heuristicFunc)
    {
        _getEdgeCost = getEdgeCost;
        _findPath = (t, r) =>
        {
            var p = PathFinder<T>.FindPathAStar(
                t, r, getNeighbors, GetEdgeCost, 
                heuristicFunc, out var cost);
            return (p, cost);
        };
    }

    public List<T> GetPath(T start, T end,
        out float cost)
    {
        var key = start.GetIdEdgeKey(end);
        var (lo, hi) = start.Id < end.Id
            ? (start, end)
            : (end, start);
        
        if(PathCache.ContainsKey(key) == false)
        {
            var (newPath, newCost) = _findPath(lo, hi);
            PathCache.Add(key, newPath);
            var reverse = new List<T>(newPath.Count);
            for (var i = newPath.Count - 1; i >= 0; i--)
            {
                reverse.Add(newPath[i]);
            }
            ReversePathCache.Add(key, reverse);
            PathCostCache.Add(key, newCost);
        }
        
        cost = PathCostCache[key];
        
        return start.Id < end.Id
            ? PathCache[key]
            : ReversePathCache[key];
    }

    public float GetEdgeCost(T t, T r)
    {
        var key = t.GetIdEdgeKey(r);
        if (EdgeCostCache.TryGetValue(key, out var edgeCost) == false)
        {
            edgeCost = _getEdgeCost(t, r);
            EdgeCostCache.Add(key, edgeCost);
        }
        
        return edgeCost;
    }
}