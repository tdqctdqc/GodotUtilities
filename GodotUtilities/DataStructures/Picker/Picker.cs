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

    
    public void RandomAgentPick(int toLeave = 0)
    {
        while (OpenPickers.Count > 0 && NotTaken.Count > toLeave)
        {
            var agent = OpenPickers.GetRandomElement();
            var open = agent.Pick(this);
            if (open == false) OpenPickers.Remove(agent);
            GD.Print($"remaining elements {NotTaken.Count}");
            GD.Print($"remaining agents {OpenPickers.Count}");
        }
    }
    
    
}