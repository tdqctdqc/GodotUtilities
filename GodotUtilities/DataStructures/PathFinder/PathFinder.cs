using Godot;
using GodotUtilities.DataStructures.Tree;
using Priority_Queue;

namespace GodotUtilities.DataStructures.PathFinder;
using GodotUtilities.DataStructures.Graph;

public static partial class PathFinder
{
    public static CachedPathFinder<T>
        FindNetworkPaths<T>
        (
            IEnumerable<T> networkElements,
            Func<T, T> getSeed,
            Func<T, IEnumerable<T>> getHiLvlNeighbors,
            Func<T, IEnumerable<T>> getLowLvlNeighbors,
            Func<T, T, float> heuristicCost,
            Func<T, T, float> getEdgeCost)
        where T : class, IIded
    {
        var pathFinder = CachedPathFinder<T>.ConstuctAStar(
            getEdgeCost,
            getLowLvlNeighbors, heuristicCost
        );
        
        foreach (var super in networkElements)
        {
            foreach (var superN in getHiLvlNeighbors(super))
            {
                pathFinder.GetPath(getSeed(super), getSeed(superN), 
                    out var cost);
            }
        }

        return pathFinder;
    }


    
}
public static class PathFinder<T>
    where T : class, IIded
{
    public static List<T> FindPathAStar(T start, T end, 
        Func<T, IEnumerable<T>> getNeighbors, 
        Func<T,T,float> getEdgeCost, 
        Func<T,T,float> heuristicFunc,
        out float cost,
        int maxIter = 10_000)
    {
        var open = new SimplePriorityQueue<PathFinderNode<T>, float>();
        var nodes = new Dictionary<T, PathFinderNode<T>>();
        var startNode = addNode(start);
        updateNode(startNode, 0f);
        
        int iter = 0;
        List<T> path = null;
        cost = float.PositiveInfinity;

        while(open.Count > 0 && iter < maxIter)
        {
            iter++;
            var current = open.Dequeue();
            if(current.Element.Equals(end))
            {
                path = BuildPathBackwards(current);
                cost = current.CostFromStart;
                break;
            }
            
            foreach (var n in getNeighbors(current.Element))
            {
                var edgeCost = getEdgeCost(current.Element, n);
                if (float.IsPositiveInfinity(edgeCost)) continue;
                if (nodes.ContainsKey(n) == false)
                {
                    addNode(n);
                }

                var nNode = nodes[n];
                var newCost = current.CostFromStart + edgeCost;
                if(newCost < nNode.CostFromStart)
                {
                    nNode.Parent = current;
                    updateNode(nNode, newCost);
                }
            }
        }

        return path;

        PathFinderNode<T> addNode(T element)
        {
            var node = new PathFinderNode<T>();
            node.HeuristicCost = heuristicFunc(element, end);
            node.CostFromStart = Mathf.Inf;
            node.Element = element;
            open.Enqueue(node, Mathf.Inf);
            nodes.Add(element, node);
            return node;
        }
        
        void updateNode(PathFinderNode<T> node, float newCostFromStart)
        {
            node.CostFromStart = newCostFromStart;
            if (open.Contains(node))
            {
                open.UpdatePriority(node, node.CostFromStart + node.HeuristicCost);
            }
            else
            {
                open.Enqueue(node, node.CostFromStart + node.HeuristicCost);
            }
        }
    }
    

    public static T FindClosest(T start, Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, float> getEdgeCost, Func<T, bool> result, out float costToClosest)
    {
        var res = new HashSet<T>();
        var open = new SimplePriorityQueue<T, float>();
        open.Enqueue(start, 0f);
        costToClosest = Single.PositiveInfinity;
        while (open.Count > 0)
        {
            var top = open.First;
            var cost = open.GetPriority(top);

            if (result(top))
            {
                costToClosest = cost;
                return top;
            }
            open.Dequeue();
            res.Add(top);
            foreach (var n in getNeighbors(top))
            {
                var edgeCost = getEdgeCost(top, n);
                var newCost = cost + edgeCost;
                if (open.Contains(n))
                {
                    var nCost = open.GetPriority(n);
                    if (newCost < nCost)
                    {
                        open.UpdatePriority(n, newCost);
                    }
                }
                else
                {
                    open.Enqueue(n, newCost);
                }
            }
        }

        return default;
    }

    public static HashSet<T> FloodFill(T start, Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, float> getEdgeCost, float maxCost)
    {
        var res = new HashSet<T>();
        var open = new SimplePriorityQueue<T, float>();
        open.Enqueue(start, 0f);
        while (open.Count > 0)
        {
            var top = open.First;
            var cost = open.GetPriority(top);
            open.Dequeue();
            res.Add(top);
            foreach (var n in getNeighbors(top))
            {
                var edgeCost = getEdgeCost(top, n);
                var newCost = cost + edgeCost;
                if (newCost > maxCost) continue;
                if (open.Contains(n))
                {
                    var nCost = open.GetPriority(n);
                    if (newCost < nCost)
                    {
                        open.UpdatePriority(n, newCost);
                    }
                }
                else
                {
                    open.Enqueue(n, newCost);
                }
            }
        }

        return res;
    }
    
    
    public static Dictionary<T, float> FloodFillGetCosts(T start, Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, float> getEdgeCost, float maxCost)
    {
        var res = new Dictionary<T, float>();
        var open = new SimplePriorityQueue<T, float>();
        open.Enqueue(start, 0f);
        while (open.Count > 0)
        {
            var top = open.First;
            var cost = open.GetPriority(top);
            open.Dequeue();
            if (res.TryGetValue(top, out var old) == false
                || cost < old)
            {
                res[top] = cost;
            }
            foreach (var n in getNeighbors(top))
            {
                var edgeCost = getEdgeCost(top, n);
                var newCost = cost + edgeCost;
                if (newCost > maxCost) continue;
                if (open.Contains(n))
                {
                    var nCost = open.GetPriority(n);
                    if (newCost < nCost)
                    {
                        open.UpdatePriority(n, newCost);
                    }
                }
                else
                {
                    open.Enqueue(n, newCost);
                }
            }
        }

        return res;
    }
    
    
    // public static CachedPathFinder<T> FindPathHierarchical<TSuper>
    //     (
    //         
    //     )
    // {
    //     
    // }
    
    
    
    
    private static List<T> BuildPathBackwards(
        PathFinderNode<T> end) 
    {
        var path = new List<T> {end.Element};
        var to = end;
        while (to.Parent is not null)
        {
            var from = to.Parent;
            path.Add(from.Element);
            to = from;
        }

        path.Reverse();
        return path;
    }
}
