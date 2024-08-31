using Godot;

namespace GodotUtilities.DataStructures.Picker;
using Priority_Queue;
public class HeuristicPickerAgent<T> : IPickerAgent<T>
{
    public SimplePriorityQueue<T, float> Candidates { get; private set; }
    public HashSet<T> Seeds { get; }
    private T _seed;
    private Func<T, bool> _valid;
    public HashSet<T> Picked { get; }
    private Func<T, T, float> _heuristic;
    public int NumToPick { get; private set; }
    public HeuristicPickerAgent(T seed,
        Picker<T> host,
        Func<T, bool> valid, Func<T, T, float> heuristic, 
        int numToPick)
    {
        _seed = seed;
        _valid = valid;
        _heuristic = heuristic;
        NumToPick = numToPick;
        Seeds = new HashSet<T>{seed};
        Picked = new HashSet<T>();
        Candidates = new SimplePriorityQueue<T, float>();
        Add(seed, host);
        host.AddAgent(this);
    }

    public IEnumerable<T> Pick(Picker<T> host)
    {
        var had = Picked.Count;
        var taken = 0;
        while (Candidates.Any() && taken < NumToPick)
        {
            var t = Candidates.Dequeue();
            if (host.NotTaken.Contains(t) && _valid(t))
            {
                taken++;
                Add(t, host);
                yield return t;
            }
        }
    }

    private void Add(T t, Picker<T> host)
    {
        Picked.Add(t);
        foreach (var n in host.GetNeighbors(t))
        {
            if (host.NotTaken.Contains(n) && _valid(n)
                && Candidates.Contains(n) == false)
            {
                var dist = _heuristic(_seed, n);
                Candidates.Enqueue(n, dist);
            }
        }
    }
}