namespace GodotUtilities.DataStructures.Graph;

public class AdjacencyGraph<TElement>
{
    public Dictionary<TElement, HashSet<TElement>> Dic { get; private set; }
    public HashSet<TElement> this[TElement e] => Dic[e];
    public AdjacencyGraph(Dictionary<TElement, HashSet<TElement>> dic)
    {
        Dic = dic;
    }

    public void AddEdge(TElement t1, TElement t2)
    {
        if (Dic.ContainsKey(t1) == false)
        {
            Dic.Add(t1, new HashSet<TElement>());
        }
        if (Dic.ContainsKey(t2) == false)
        {
            Dic.Add(t2, new HashSet<TElement>());
        }

        Dic[t1].Add(t2);
        Dic[t2].Add(t1);
    }
}