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
            
            var n = ns.MinBy(x => dist(seed, x));
            open.Add(n);
            res.Add(n);
        }

        return res;
    }
    public static HashSet<T> FloodFillHeuristicToLimitQueue(T seed,
        int limit,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> valid,
        Func<T, T, float> heuristic)
    {
        var res = new HashSet<T> { };
        var open = new SimplePriorityQueue<T, float>();
        open.Enqueue(seed, heuristic(seed, seed));
        while (open.Count > 0 && res.Count < limit)
        {
            var curr = open.Dequeue();
            if (valid(curr) == false) continue;
            res.Add(curr);
            foreach (var n in getNeighbors(curr))
            {
                if (res.Contains(n)
                    || open.Contains(n)
                    || valid(n) == false)
                {
                    continue;
                }

                var dist = heuristic(n, seed);
                open.Enqueue(n, dist);
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
    
    public static Dictionary<T, List<T>> FloodFillMultiple
    (IEnumerable<T> seeds, Func<T, IEnumerable<T>> getNs,
        Func<T, T, float> getHeuristic,
        HashSet<T> free)
    {
        var seedsQueue = new PriorityQueue<T, int>();
        foreach (var seed in seeds)
        {
            seedsQueue.Enqueue(seed, 1);
        }
        var res = seeds.ToDictionary(s => s, 
            s => new List<T>{s});

        while (seedsQueue.Count > 0)
        {
            var seed = seedsQueue.Dequeue();
            var set = res[seed];
            var freeNs = set.SelectMany(s => getNs(s))
                .Where(free.Contains)
                .OrderBy(t => getHeuristic(seed, t));
            if (freeNs.Any() == false)
            {
                continue;
            }

            var take = freeNs.First();
            free.Remove(take);
            set.Add(take);
            seedsQueue.Enqueue(seed, set.Count);
        }

        if (free.Count > 0) throw new Exception();
        return res;
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

    public static HashSet<T> FloodTilFirst(T start, 
        Func<T, bool> validNeighbor,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, bool> validResult,
        int maxIter = 1_000)
    {
        var res = new HashSet<T>{start};
        var queue = new Queue<T>();
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
                    return res;
                }
                if (res.Contains(neighbor)) continue;
                if (validNeighbor(neighbor) == false) continue;
                queue.Enqueue(neighbor);
                res.Add(neighbor);
            }
        }

        return res;
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
}