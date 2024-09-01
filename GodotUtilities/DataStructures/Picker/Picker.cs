using Godot;

namespace GodotUtilities.DataStructures.Picker;

public class Picker<T>
{
    public HashSet<T> NotTaken { get; private set; }
    public HashSet<IPickerAgent<T>> OpenPickers { get; private set; }
    public List<IPickerAgent<T>> Agents { get; private set; }
    public Func<T, IEnumerable<T>> GetNeighbors { get; private set; }
    public Picker(IEnumerable<T> notTaken,
        Func<T, IEnumerable<T>> getNeighbors)
    {
        GetNeighbors = getNeighbors;
        NotTaken = notTaken.EnumerableToHashSet();
        OpenPickers = new HashSet<IPickerAgent<T>>();
        Agents = new List<IPickerAgent<T>>();
    }

    public void AddAgent(IPickerAgent<T> w)
    {
        NotTaken.RemoveWhere(w.Picked.Contains);
        OpenPickers.Add(w);
        Agents.Add(w);
    }

    

    public void PickRandomlyToLimit(int toLeave = 0)
    {
        while (OpenPickers.Count > 0 && NotTaken.Count > toLeave)
        {
            RandomAgentPick();
        }
    }

    public void PickInOrderTil(int toLeave = 0)
    {
        while (OpenPickers.Count > 0 && NotTaken.Count > toLeave)
        {
            EachAgentPickOnce();
        }
    }
    public void EachAgentPickOnce(int toLeave = 0)
    {
        foreach (var agent in OpenPickers.ToArray())
        {
            AgentPick(agent);
            if (NotTaken.Count <= toLeave) break;
        }
    }
    public void RandomAgentPick()
    {
        var agent = OpenPickers.GetRandomElement();
        AgentPick(agent);
    }

    public void AgentPick(IPickerAgent<T> agent)
    {
        if (OpenPickers.Contains(agent) == false) return;
        var picked = agent.Pick(this);
        var found = false;
        foreach (var v in picked)
        {
            found = true;
            NotTaken.Remove(v);
        }
            
        if (found == false) OpenPickers.Remove(agent);
    }
    
    
}