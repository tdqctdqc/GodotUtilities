namespace GodotUtilities.DataStructures.Graph;

public interface IReadOnlyGraph<TNode>
{
    IEnumerable<TNode> Elements { get; }
    bool HasEdge(TNode from, TNode to);
    bool HasNode(TNode value);
    IEnumerable<TNode> GetNeighbors(TNode value);
}
public interface IReadOnlyGraph<TNode, TEdge> 
    : IReadOnlyGraph<TNode>
{
    TEdge GetEdge(TNode from, TNode to);
}