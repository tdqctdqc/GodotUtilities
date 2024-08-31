namespace GodotUtilities.DataStructures.PathFinder;

public class PathFinderNode<T>
{
    public T Element { get; set; }
    public PathFinderNode<T> Parent { get; set; }
    public float CostFromStart { get; set; }
    public float HeuristicCost { get; set; }

    public void Clear()
    {
        Parent = default;
        Element = default;
        CostFromStart = 0f;
        HeuristicCost = 0f;
    }
}