using System.Collections.Specialized;
using Godot;
using Priority_Queue;

namespace GodotUtilities.DataStructures;

public static class FloodFill<T>
{
    public static HashSet<T> FloodFillRandomishToLimit(T seed,
        int limit,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> valid,
        RandomNumberGenerator random)
    {
        var res = new HashSet<T> { seed };
        var open = new HashSet<T> { seed };
        while (res.Count < limit)
        {
            if (open.Count == 0) break;
            var curr = open.GetRandomElement();
            var ns = getNeighbors(curr)
                .Where(x => res.Contains(x) == false 
                    && valid(x));
            if (ns.Any() == false)
            {
                open.Remove(curr);
                continue;
            }

            var n = ns.GetRandomElement();
            open.Add(n);
            res.Add(n);
        }

        return res;
    }
    
    public static HashSet<T> FloodFillHeuristicToLimit(T seed,
        int limit,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> valid,
        Func<T, T, float> dist)
    {
        var res = new HashSet<T> { seed };
        var open = new SimplePriorityQueue<T, float>();
        open.Enqueue(seed, dist(seed, seed));
        while (res.Count < limit)
        {
            if (open.Count == 0) break;
            var curr = open.Dequeue();
            res.Add(curr);
            
            var ns = getNeighbors(curr)
                .Where(x => res.Contains(x) == false 
                            && valid(x));

            foreach (var n in ns)
            {
                if (open.Contains(n) == false)
                {
                    open.Enqueue(n, dist(seed, n));
                }
            }
        }

        return res;
    }
    
    public static HashSet<T> FloodFillToLimit(T seed,
        int limit,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> valid)
    {
        var list = new List<T>{ seed };
        var res = new HashSet<T> { seed };
        var lastAddition = 1;
        
        while(res.Count < limit)
        {
            var thisAddition = 0;
            var count = list.Count;
            for (int j = count - lastAddition; j < count; j++)
            {
                var t = list[j];
                foreach (var n in getNeighbors(t))
                {
                    if (res.Contains(n) == false && valid(n))
                    {
                        list.Add(n);
                        res.Add(n);
                        thisAddition++;
                        if (res.Count == limit) break;
                    }
                }
            }

            if (thisAddition == 0) break;
            lastAddition = thisAddition;
        }

        return res;
    }
    
    
    public static HashSet<T> FloodFillToRadius(T seed,
        int radius,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> valid)
    {
        var last = new List<T> { seed };
        var res = new HashSet<T> { seed };
        var lastAddition = 1;
        for (var i = 0; i < radius; i++)
        {
            var thisAddition = 0;
            var count = last.Count;
            for (int j = count - lastAddition; j < count; j++)
            {
                var t = last[j];
                foreach (var n in getNeighbors(t))
                {
                    if (res.Contains(n) == false && valid(n))
                    {
                        last.Add(n);
                        res.Add(n);
                        thisAddition++;
                    }
                }
            }

            if (thisAddition == 0) return res;
            lastAddition = thisAddition;
        }

        return res;
    }
    
    public static T FloodFillToRadiusTilFirst(T seed,
        int radius,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> valid, out HashSet<T> flood,
        out int distance)
    {
        var last = new List<T> { seed };
        var res = new HashSet<T> { seed };
        flood = res;
        var lastAddition = 1;
        distance = 0;
        for (var i = 0; i < radius; i++)
        {
            distance = i + 1;
            var thisAddition = 0;
            var count = last.Count;
            for (int j = count - lastAddition; j < count; j++)
            {
                var t = last[j];
                foreach (var n in getNeighbors(t))
                {
                    if (res.Contains(n) == false && valid(n))
                    {
                        last.Add(n);
                        res.Add(n);
                        thisAddition++;
                        if (valid(n))
                        {
                            return n;
                        }
                    }
                }
            }

            if (thisAddition == 0) return default;
            lastAddition = thisAddition;
        }

        return default;
    }
    
    public static HashSet<T> GetFloodFill(T start, 
        Func<T, bool> valid,
        Func<T, IEnumerable<T>> getNeighbors)
    {
        var res = new HashSet<T>{start};
        var queue = new Queue<T>();
        queue.Enqueue(start);
        while (queue.TryDequeue(out var curr))
        {
            var neighbors = getNeighbors(curr);
            foreach (var neighbor in neighbors)
            {
                if (res.Contains(neighbor)) continue;
                if (valid(neighbor) == false) continue;
                queue.Enqueue(neighbor);
                res.Add(neighbor);
            }
        }
        
        return res;
    }

    public static T FloodTilFirst(T start, 
        Func<T, bool> validNeighbor,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> validResult,
        out HashSet<T> flood,
        int maxIter = 1_000)
    {
        var res = new HashSet<T>{start};
        var queue = new Queue<T>();
        flood = res;
        queue.Enqueue(start);
        int iter = 0;
        while (queue.TryDequeue(out var curr))
        {
            iter++;
            if (iter == maxIter) break;
            var neighbors = getNeighbors(curr);
            foreach (var neighbor in neighbors)
            {
                
                if (res.Contains(neighbor)) continue;
                if (validNeighbor(neighbor) == false) continue;
                queue.Enqueue(neighbor);
                res.Add(neighbor);
                if (validResult(neighbor))
                {
                    return neighbor;
                }
            }
        }

        return default;
    }
    
    public static T FindFirst(T start, 
        Func<T, bool> validNeighbor,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> validResult,
        int maxIter = 1_000)
    {
        var queue = new Queue<T>();
        var covered = new HashSet<T>{start};
        queue.Enqueue(start);
        int iter = 0;
        while (queue.TryDequeue(out var curr))
        {
            iter++;
            if (iter == maxIter) break;
            var neighbors = getNeighbors(curr);
            foreach (var neighbor in neighbors)
            {
                if (validResult(neighbor))
                {
                    return neighbor;
                }
                if (covered.Contains(neighbor)) continue;
                covered.Add(neighbor);
                if (validNeighbor(neighbor) == false)
                {
                    continue;
                }
                queue.Enqueue(neighbor);
            }
        }

        throw new Exception();
    }

    public static HashSet<T> FloodFillTilAllFoundMultipleStarts
    (IEnumerable<T> starts, 
        Func<T, bool> validNeighbor,
        Func<T, IEnumerable<T>> getNeighbors,
        IEnumerable<T> need,
        int maxIter = 1_000)
    {
        var res = starts.EnumerableToHashSet();
        var unencountered = need.EnumerableToHashSet();
        var queue = new Queue<T>();
        foreach (var r in res)
        {
            queue.Enqueue(r);
        }
        int iter = 0;
        while (queue.TryDequeue(out var curr))
        {
            iter++;
            if (iter == maxIter) break;
            var neighbors = getNeighbors(curr);
            foreach (var neighbor in neighbors)
            {
                if (unencountered.Contains(neighbor))
                {
                    unencountered.Remove(neighbor);
                }
                if (res.Contains(neighbor)) continue;
                if (validNeighbor(neighbor) == false) continue;
                queue.Enqueue(neighbor);
                res.Add(neighbor);
                if (unencountered.Count == 0) return res;
            }
        }

        return res;
    }


    public static Dictionary<T, float> FindMinDistMap(IEnumerable<T> seeds,
        Func<T, T, float> dist, Func<T, IEnumerable<T>> getNeighbors,
        float maxDist = Single.PositiveInfinity)
    {
        var open = new SimplePriorityQueue<T, float>();

        var res = new Dictionary<T, float>();
        foreach (var seed in seeds)
        {
            open.Enqueue(seed, dist(seed, seed));
        }

        while (open.Count > 0)
        {
            var priority = open.GetPriority(open.First);
            var curr = open.Dequeue();
            res.Add(curr, priority);
            foreach (var n in getNeighbors(curr))
            {
                if (res.ContainsKey(n)) continue;
                var edgeCost = dist(curr, n);
                var nCost = edgeCost + priority;
                if (nCost >= maxDist)
                {
                    continue;
                }
                if (open.Contains(n) == false)
                {
                    open.Enqueue(n, nCost);
                }
                else if (open.GetPriority(n) > nCost)
                {
                    open.UpdatePriority(n, nCost);
                }
            }
        }

        return res;
    }
}