namespace GodotUtilities.DataStructures.PathFinder;

public class PathFindInfo<T>
{
    public Dictionary<T, float> CostsFromStart;
    public Dictionary<T, float> HeuristicCosts;
    public Priority_Queue.SimplePriorityQueue<T, float> Open;
    public Dictionary<T, T> Parents;

    public PathFindInfo()
    {
        Open = new Priority_Queue.SimplePriorityQueue<T, float>();
        CostsFromStart = new Dictionary<T, float>();
        HeuristicCosts = new Dictionary<T, float>();
        Parents = new Dictionary<T, T>();
    }

    public void Clear()
    {
        Open.Clear();
        CostsFromStart.Clear();
        Parents.Clear();
    }
}