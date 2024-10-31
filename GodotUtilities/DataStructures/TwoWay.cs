namespace GodotUtilities.DataStructures;

public class TwoWay<T, R>
{
    public Dictionary<T, HashSet<R>> RsByT { get; private set; }
    public Dictionary<R, HashSet<T>> TsByR { get; private set; }
    public TwoWay(Dictionary<R, HashSet<T>> tsByR, Dictionary<T, HashSet<R>> rsByT)
    {
        TsByR = tsByR;
        RsByT = rsByT;
    }

    
    
    public void AddOrUpdate(T t, R r)
    {
        if (RsByT.ContainsKey(t) == false)
        {
            RsByT.Add(t, new HashSet<R>());
        }
        if (TsByR.ContainsKey(r) == false)
        {
            TsByR.Add(r, new HashSet<T>());
        }
        RsByT[t].Add(r);
        TsByR[r].Add(t);
    }

    public void Remove(T t)
    {
        if (RsByT.TryGetValue(t, out var oldRs))
        {
            foreach (var oldR in oldRs)
            {
                RemoveTFromR(t, oldR);
            }
        }

        RsByT.Remove(t);
    }
    private void AddTToR(T t, R r)
    {
        TsByR.AddOrUpdate(r, t);
    }

    private void RemoveTFromR(T t, R r)
    {
        TsByR[r].Remove(t);
    }
}