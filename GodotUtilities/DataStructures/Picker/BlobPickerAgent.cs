namespace GodotUtilities.DataStructures.Picker;

public class BlobPickerAgent<T> : IPickerAgent<T>
{
    public HashSet<T> Seeds { get; private set; }
    public HashSet<T> Picked { get; private set; }
    public HashSet<T> Frontier { get; private set; }
    public HashSet<T> Take { get; private set; }
    private Func<T, bool> _valid;

    public BlobPickerAgent(T seed, Picker<T> host, 
        Func<T, bool> valid)
    {
        Seeds = new HashSet<T> { seed };
        _valid = valid;
        Picked = new HashSet<T>();
        Frontier = new HashSet<T>();
        Take = new HashSet<T>();
        foreach (var seed1 in Seeds)
        {
            Frontier.Add(seed1);
        }
        Add(seed, host);
        host.AddAgent(this);
    }

    public IEnumerable<T> Pick(Picker<T> host)
    {
        Take.Clear();
        foreach (var f in Frontier)
        {
            foreach (var n in host.GetNeighbors(f))
            {
                if (host.NotTaken.Contains(n) && _valid(n))
                {
                    Add(n, host);
                    Take.Add(n);
                }
            }
        }
        
        (Frontier, Take) = (Take, Frontier);
        return Frontier;
    }

    protected void Add(T t, Picker<T> host)
    {
        Picked.Add(t);
    }

}