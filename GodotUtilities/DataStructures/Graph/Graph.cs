namespace GodotUtilities.DataStructures.Graph;

public class Graph<TElement, TEdge> : IGraph<TElement, TEdge>
{
    public IGraphNode<TElement, TEdge> this[TElement t] => _nodeDic[t];
    private Dictionary<TElement, IGraphNode<TElement, TEdge>> _nodeDic;
    public List<TElement> Elements { get; private set; }
    
    public List<GraphNode<TElement, TEdge>> Nodes { get; private set; }
    public HashSet<TEdge> Edges { get; private set; }
    public Graph()
    {
        _nodeDic = new Dictionary<TElement, IGraphNode<TElement, TEdge>>();
        Elements = new List<TElement>();
        Nodes = new List<GraphNode<TElement, TEdge>>();
        Edges = new HashSet<TEdge>();
    }
    
    public bool HasEdge(TElement t1, TElement t2)
    {
        if (_nodeDic.ContainsKey(t1) == false) return false;
        return _nodeDic[t1].HasNeighbor(t2);
    }

    public void ForEachEdge(Action<TElement, TElement, TEdge> action)
    {
        foreach (var element in Elements)
        {
            foreach (var neighbor in GetNeighbors(element))
            {
                var edge = GetEdge(element, neighbor);
                action(element, neighbor, edge);
            }
        }
    }
    public void RemoveEdge(TElement t1, TElement t2)
    {
        var edge = GetEdge(t1, t2);
        var n1 = _nodeDic[t1];
        var n2 = _nodeDic[t2];
        n1.RemoveNeighbor(t2);
        n2.RemoveNeighbor(t1);
        Edges.Remove(edge);
    }
    public void RemoveEdgesWhere(Func<TEdge, bool> remove)
    {
        foreach (var el in Elements)
        {
            var ns = GetNeighbors(el).ToArray();
            foreach (var n in ns)
            {
                var edge = GetEdge(el, n);
                if (remove(edge))
                {
                    RemoveEdge(el, n);
                }
            }
        }
    }

    IEnumerable<TElement> IReadOnlyGraph<TElement>.GetNeighbors(TElement value)
    {
        return GetNeighbors(value);
    }

    public IGraphNode<TElement> GetNode(TElement t)
    {
        return _nodeDic[t];
    }

    public void SetEdgeValue(TElement t1, TElement t2, TEdge newEdgeVal)
    {
        var node1 = _nodeDic[t1];
        var node2 = _nodeDic[t2];
        node1.SetEdgeValue(node2, newEdgeVal);
        node2.SetEdgeValue(node1, newEdgeVal);
    }
    public void AddEdge(TElement t1, TElement t2, TEdge edge)
    {
        if (t1 == null || t2 == null) return;
        if(_nodeDic.ContainsKey(t1) == false) AddNode(t1);
        if(_nodeDic.ContainsKey(t2) == false) AddNode(t2);
        AddUndirectedEdge(t1, t2, edge);
    }
    private void AddUndirectedEdge(TElement from, TElement to, TEdge edge)
    {
        Edges.Add(edge);
        var fromNode = _nodeDic[from];
        var toNode = _nodeDic[to];
        fromNode.AddNeighbor(to, edge);
        toNode.AddNeighbor(from, edge);
    }
    public TEdge GetEdge(TElement t1, TElement t2)
    {
        var node1 = _nodeDic[t1];
        return node1.GetEdge(t2);
    }
    public void AddNode(GraphNode<TElement, TEdge> node)
    {
        _nodeDic.Add(node.Element, node);
        Elements.Add(node.Element);
        Nodes.Add(node);
    }
    public void AddNode(TElement element)
    {
        var node = new GraphNode<TElement, TEdge>(element);
        _nodeDic.Add(node.Element, node);
        Elements.Add(element);
        Nodes.Add(node);
    }
    
    public void AddUndirectedEdge(IGraphNode<TElement, TEdge> from, 
        IGraphNode<TElement, TEdge> to, TEdge edge)
    {
        Edges.Add(edge);
        from.AddNeighbor(to.Element, edge);
        to.AddNeighbor(from.Element, edge);
    }
    public void Remove(TElement value)
    {
        IGraphNode<TElement, TEdge> nodeToRemove = _nodeDic[value];
        if (nodeToRemove == null) return;
        Elements.Remove(value);
        _nodeDic.Remove(nodeToRemove.Element);

        foreach (var neighbor in nodeToRemove.Neighbors)
        {
            var nNode = _nodeDic[neighbor];
            nNode.RemoveNeighbor(nodeToRemove.Element);
        }
    }

    public HashSet<TElement> GetNeighbors(TElement value)
    {
        //todo make igraphnode neighbors hashset?
        return _nodeDic[value].Neighbors.ToHashSet();
    }
    public bool HasNode(TElement value)
    {
        return _nodeDic.ContainsKey(value);
    }
    IEnumerable<TElement> IReadOnlyGraph<TElement>.Elements => Elements;
}