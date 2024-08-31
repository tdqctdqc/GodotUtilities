namespace GodotUtilities.DataStructures.Picker;
using Priority_Queue;
public class AdjacencyCountPickerAgent<T> : IPickerAgent<T>
{
    public HashSet<T> Seeds { get; private set; }
    public HashSet<T> Picked { get; private set; }
    public SimplePriorityQueue<T> Adjacents { get; private set; }
    private Func<T, bool> _valid;
    public int NumToPick { get; private set; }

    public AdjacencyCountPickerAgent(T seed, 
        Picker<T> host, 
        int numToPick, 
        Func<T, bool> valid)
    {
        Seeds = new HashSet<T> { seed };
        _valid = valid;
        NumToPick = numToPick;
        Picked = new HashSet<T>();
        Adjacents = new SimplePriorityQueue<T>();
        host.AddAgent(this);
        Add(seed, host);
    }

    public IEnumerable<T> Pick(Picker<T> host)
    {
        var taken = 0;
        while (Adjacents.Any() && taken < NumToPick)
        {
            var t = Adjacents.Dequeue();
            if (host.NotTaken.Contains(t))
            {
                taken++;
                Add(t, host);
                yield return t;
            }
        }
    }

    

    protected void Add(T t, Picker<T> host)
    {
        Picked.Add(t);
        Adjacents.Remove(t);
        
        foreach (var n in host.GetNeighbors(t))
        {
            if (host.NotTaken.Contains(n) && _valid(n) 
                && Picked.Contains(n) == false)
            {
                if (Adjacents.Contains(n))
                {
                    var score = Adjacents.GetPriority(n);
                    Adjacents.UpdatePriority(n, score - 1f);
                }
                else
                {
                    Adjacents.Enqueue(n, -1f);
                }
            }
        }
    }

}