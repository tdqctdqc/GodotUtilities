namespace GodotUtilities.DataStructures.Tree;

public interface IAggregate<TSelf, TSub>
    : INeighbored<TSelf>, IChildrened<TSub>
{
    TSub Seed { get; }
    void SetSeed(TSub seed);
}