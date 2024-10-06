using System.Collections.Concurrent;
using System.Diagnostics;
using Godot;
using GodotUtilities.GameData;

namespace GodotUtilities.Logger;

public class Logger
{
    private Data _data;
    public Dictionary<LogType, Dictionary<int, TickLogs>> Entries { get; private set; }
    private ConcurrentQueue<(int, LogType, Func<Node>)> _queue;
    private Func<Data, int> _getTick;
    public Logger(Data data, Func<Data, int> getTick)
    {
        _data = data;
        _getTick = getTick;
        Entries = new Dictionary<LogType, Dictionary<int, TickLogs>>();
        _queue = new ConcurrentQueue<(int, LogType, Func<Node>)>();
        RunLoop();
    }

    private async void RunLoop()
    {
        await Task.Run(Loop);
    }
    
    private void Loop()
    {
        while (true)
        {
            while (_queue.TryDequeue(out var res))
            {
                var entries = Entries.GetOrAdd(res.Item2, i => new Dictionary<int, TickLogs>());
                var entry = entries.GetOrAdd(res.Item1, t => new TickLogs(t));
                entry.Logs.Add(res.Item3);
            }
        }
    }
    public void Log(int tick, string msg, LogType logType)
    {
        _queue.Enqueue((tick, logType, () => NodeExt.CreateLabel(msg)));
    }

    public void RunAndLogTime(int tick, string name, LogType type, Action a)
    {
        var sw = new Stopwatch();
        sw.Start();
        a.Invoke();
        sw.Stop();
        var ms = sw.Elapsed.TotalMilliseconds;
        Log(tick, $"{name}: {ms} ms", type);
    }
    
    public T RunAndLogTime<T>(int tick, string name, LogType type, Func<T> a)
    {
        var sw = new Stopwatch();
        sw.Start();
        var t = a.Invoke();
        sw.Stop();
        var ms = sw.Elapsed.TotalMilliseconds;
        Log(tick, $"{name}: {ms} ms", type);
        return t;
    }
}