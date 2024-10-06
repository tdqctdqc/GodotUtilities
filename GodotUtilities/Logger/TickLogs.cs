using Godot;

namespace GodotUtilities.Logger;

public class TickLogs
{
    public int Tick { get; private set; }
    public List<Func<Node>> Logs { get; private set; }

    public TickLogs(int tick)
    {
        Tick = tick;
        Logs = new List<Func<Node>>();
    }
}