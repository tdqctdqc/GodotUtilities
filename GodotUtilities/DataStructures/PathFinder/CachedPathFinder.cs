using Godot;

namespace GodotUtilities.DataStructures.PathFinder;

public class CachedPathFinder<T>
    where T : class, IIded
{
    public Dictionary<Vector2I, float> EdgeCostCache { get; private set; }
        = new ();
    public Dictionary<Vector2I, List<T>> PathCache { get; private set; }
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

    public List<T> FindPath(T start, T end,
        out float cost)
    {
        var key = start.GetIdEdgeKey(end);
        List<T> path;
        if (PathCache.TryGetValue(key, out var p))
        {
            (path, cost) = (p, PathCostCache[key]);
        }
        else
        {
            (path, cost) = _findPath(start, end);
            PathCache.Add(key, path);
            PathCostCache.Add(key, cost);
        }
        
        return path;
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