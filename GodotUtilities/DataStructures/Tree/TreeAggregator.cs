using Godot;
using GodotUtilities.DataStructures.Picker;

namespace GodotUtilities.DataStructures.Tree;

public static class TreeAggregator
{
    public static List<TAgg> BuildTiers<TAgg>
        (IEnumerable<TAgg> aggs, 
            Func<Func<HashSet<TAgg>, IEnumerable<TAgg>>> getSeedFactory,
            Func<Func<TAgg, HashSet<TAgg>, HashSet<TAgg>>> chooseFactory,
            Func<TAgg, TAgg, bool> canMerge, 
            Func<TAgg> construct,
            int consolidateIter, int min)
                where TAgg : IAggregate<TAgg, TAgg>
    {
        var curr = aggs.ToList();
        
            
        for (var i = 0; i < consolidateIter; i++)
        {

            var getSeeds = getSeedFactory();
            var choose = chooseFactory();
                
            var next = TreeAggregator.Aggregate<TAgg, TAgg>(
                getSeeds,
                choose,
                curr.ToHashSet(),
                construct,
                b => b.Neighbors
            );

            curr = next;
            var c = TreeAggregator
                .Consolidate<TAgg, TAgg>(
                    curr, min, canMerge);
            curr.RemoveAll(c.Contains);
        }
            
            
        return curr;
    }
    public static HashSet<TAgg> Consolidate<TAgg, TSub>(
        IEnumerable<TAgg> aggs, int minSize,
        Func<TAgg, TAgg, bool> canMerge)
            where TAgg : IAggregate<TAgg, TSub>
    {
        var consolidated = new HashSet<TAgg>();
        foreach (var agg in aggs)
        {
            if (agg.Children.Count() >= minSize) continue;
            var first = agg.Neighbors
                .Where(n => canMerge(agg, n))
                .OrderByDescending(n => n.Children.Count())
                .FirstOrDefault();
            
            if (first is not null)
            {
                consolidated.Add(agg);
                first.RemoveNeighbor(agg);
                foreach (var child in agg.Children.ToArray())
                {
                    first.AddChild(child);
                    // agg.RemoveChild(child);
                }
                
                foreach (var n in agg.Neighbors)
                {
                    if (n.Equals(first) == false)
                    {
                        n.RemoveNeighbor(agg);
                        n.AddNeighbor(first);
                        first.AddNeighbor(n);
                    }
                }
            }
        }

        return consolidated;
    }
    
    
    public static List<TAgg> Aggregate<TSub, TAgg>
    (   Func<HashSet<TSub>, IEnumerable<TSub>> getSeeds,
        Func<TSub, HashSet<TSub>, HashSet<TSub>> choose,
        HashSet<TSub> elements, 
        Func<TAgg> construct,
        Func<TSub, IEnumerable<TSub>> getNeighbors) 
        where TAgg : IAggregate<TAgg, TSub>
    {
        var initialCount = elements.Count;
        var aggs = new List<TAgg>();
        var dic = new Dictionary<TSub, TAgg>();
        
        while(elements.Count > 0)
        {
            var seeds = getSeeds(elements).ToArray();
            elements.ExceptWith(seeds);
            foreach (var seed in seeds)
            {
                var chosen = choose(seed, elements);
                chosen.Add(seed);
                elements.ExceptWith(chosen);
                var agg = construct();
                foreach (var t in chosen)
                {
                    agg.AddChild(t);
                    dic.Add(t, agg);
                }
                aggs.Add(agg);
            }
        }

        if (dic.Count != initialCount)
        {
            throw new Exception();
        }
        foreach (var agg in aggs)
        {
            var nAggs = 
                agg.Children
                    .SelectMany(getNeighbors)
                    .Distinct()
                    .Select(nSub => dic[nSub]);
            foreach (var nAgg in nAggs)
            {
                if (nAgg.Equals(agg) == false)
                {
                    agg.AddNeighbor(nAgg);
                }
            }
        }
        return aggs;
    }
}