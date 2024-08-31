namespace GodotUtilities.DataStructures.Picker;

public class BlobPickerAgent<T> : IPickerAgent<T>
{
    public HashSet<T> Seeds { get; private set; }
    public HashSet<T> Picked { get; private set; }
    public HashSet<T> Frontier { get; private set; }
    public HashSet<T> FrontierBuffer { get; private set; }
    private Func<T, bool> _valid;

    public BlobPickerAgent(T seed, Picker<T> host, 
        Func<T, bool> valid)
    {
        Seeds = new HashSet<T> { seed };
        _valid = valid;
        Picked = new HashSet<T>();
        Frontier = new HashSet<T>();
        FrontierBuffer = new HashSet<T>();
        foreach (var seed1 in Seeds)
        {
            foreach (var n in host.GetNeighbors(seed1))
            {
                if (_valid(n) && host.NotTaken.Contains(n))
                {
                    Frontier.Add(n);
                }
            }
        }
        Add(seed, host);
        host.AddAgent(this);
    }

    public IEnumerable<T> Pick(Picker<T> host)
    {
        var found = false;
        FrontierBuffer.Clear();
        foreach (var x in Frontier)
        {
            if (host.NotTaken.Contains(x) && _valid(x))
            {
                found = true;
                Add(x, host);
                FrontierBuffer.UnionWith(host.GetNeighbors(x));
            }
        }
        
        (Frontier, FrontierBuffer) = (FrontierBuffer, Frontier);
        return Frontier;
    }

    protected void Add(T t, Picker<T> host)
    {
        Picked.Add(t);
    }

}