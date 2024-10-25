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

    public void AddOrUpdate(T t, IEnumerable<R> elements)
    {
        var rHash = elements.ToHashSet();
        if (RsByT.TryGetValue(t, out var oldRs))
        {
            var newRs = rHash.Except(oldRs);
            var obsolete = oldRs.Except(rHash);
            foreach (var newR in newRs)
            {
                AddTToR(t, newR);
            }

            foreach (var obsoleteR in obsolete)
            {
                RemoveTFromR(t, obsoleteR);
            }
        }

        RsByT[t] = rHash;
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