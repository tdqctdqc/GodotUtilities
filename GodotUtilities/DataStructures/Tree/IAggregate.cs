namespace GodotUtilities.DataStructures.Tree;

public interface IAggregate<TSelf, TSub>
    : INeighbored<TSelf>, IChildrened<TSub>
{
}