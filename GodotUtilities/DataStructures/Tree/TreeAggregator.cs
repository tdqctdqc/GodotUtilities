using Godot;
using GodotUtilities.DataStructures.Picker;

namespace GodotUtilities.DataStructures.Tree;

public static class TreeAggregator
{
    public static List<TAgg> BuildTiers<TAgg>
        (IEnumerable<TAgg> aggs, 
            Func<Func<HashSet<TAgg>, IEnumerable<TAgg>>> seedFactoryFactory,
            Func<IEnumerable<TAgg>, Picker<TAgg>> pickerFactory,
            Func<TAgg, Picker<TAgg>, IPickerAgent<TAgg>> agentFactory,
            Func<TAgg, TAgg, bool> canMerge, 
            Func<TAgg, TAgg> construct,
            int consolidateIter, int min)
                where TAgg : IAggregate<TAgg, TAgg>
    {
        var curr = aggs.ToList();
            
        for (var i = 0; i < consolidateIter; i++)
        {
            var seedFactory = seedFactoryFactory();
            var picker = pickerFactory(curr);
                
            var next = TreeAggregator
                .Aggregate<TAgg, TAgg>(
                    seedFactory,
                    picker,
                    agg => agentFactory(agg, picker),
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
                .OrderBy(n => n.Children.Count())
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
    (   
        Func<HashSet<TSub>, IEnumerable<TSub>> seedFactory,
        Picker<TSub> picker, 
        Func<TSub, IPickerAgent<TSub>> agentFactory,
        Func<TSub, TAgg> constructAgg,
        Func<TSub, IEnumerable<TSub>> getNeighbors) 
            where TAgg : IAggregate<TAgg, TSub>
    {
        var aggs = new List<TAgg>();
        var subLookup = new Dictionary<TSub, TAgg>();
        
        while(picker.NotTaken.Count > 0)
        {
            // GD.Print("iter " + picker.NotTaken.Count);
            var seeds = seedFactory(picker.NotTaken).ToArray();
            // GD.Print("seeds " + seeds.Length);
            
            foreach (var seed in seeds)
            {
                var agent = agentFactory(seed);
                picker.AgentPick(agent);
            }
        }
        
        foreach (var agent in picker.Agents)
        {
            var agg = constructAgg(agent.Seeds.First());
            aggs.Add(agg);
            foreach (var sub in agent.Picked)
            {
                subLookup.Add(sub, agg);
                agg.AddChild(sub);
            }
        }
        
        foreach (var agg in aggs)
        {
            var nAggs = 
                agg.Children
                    .SelectMany(getNeighbors)
                    .Distinct()
                    .Select(nSub => subLookup[nSub]);
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