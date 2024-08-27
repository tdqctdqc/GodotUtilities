namespace GodotUtilities.DataStructures.Graph;

public interface IGraphNode<TNode> : IReadOnlyGraphNode<TNode>
{
}
public interface IGraphNode<TNode, TEdge> : IGraphNode<TNode>, IReadOnlyGraphNode<TNode, TEdge>
{
    void SetEdgeValue(IGraphNode<TNode, TEdge> neighbor, TEdge edge);
    void AddNeighbor(TNode neighbor, TEdge edge);
    void RemoveNeighbor(TNode neighbor);
}