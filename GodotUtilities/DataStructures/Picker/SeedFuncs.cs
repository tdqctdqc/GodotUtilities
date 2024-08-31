using System.Collections.Immutable;
using Godot;

namespace GodotUtilities.DataStructures.Picker;

public static class SeedFuncs
{
    public static Func<HashSet<T>, IEnumerable<T>> NoNeighborSeeds<T>(
        Func<T, T, bool> canShare,
        Func<T, IEnumerable<T>> getNeighbors,
        int excludeRadius)
    {
        List<T> getList(HashSet<T> ts)
        {
            var l = new List<T>();
            var hash = ts.ToHashSet();
            while (hash.Count > 0)
            {
                var t = hash.First();
                hash.Remove(t);
                l.Add(t);
                var flood = FloodFill<T>.FloodFillToRadius(
                    t, excludeRadius, getNeighbors, 
                    n => canShare(t, n) 
                         && ts.Contains(n)
                         && hash.Contains(n));
                hash.ExceptWith(flood);
                ts.Remove(t);
            }
                
            return l;
        }
        return elements =>
        {
            if (elements.Count == 0)
            {
                GD.Print($"out of seeds");
                return ImmutableArray<T>.Empty;
            }
            return getList(elements);
        };
    }
    
    public static Func<(T, bool)> PreselectedSeeds<T>(IEnumerable<T> seeds,
        HashSet<T> elements)
    {
        var list = seeds.ToList();
        elements.ExceptWith(list);
        int iter = 0;
        return () =>
        {
            if (iter < list.Count)
            {
                var v = (list[iter], true);
                iter++;
                return v;
            }

            return (default, false);
        };
    }

    public static Func<HashSet<T>, IEnumerable<T>> 
        GetFirstRemainingSeed<T>()
    {
        return elements =>
        {
            if (elements.Count > 0)
            {
                var first = elements.First();
                elements.Remove(first);
                return first.Yield();
            }
            return ImmutableArray<T>.Empty;
        };
    }

}