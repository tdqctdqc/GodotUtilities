using System.Collections.Concurrent;
using Godot;
using GodotUtilities.DataStructures.RefAction;

namespace GodotUtilities.GameClient;

public class GameClient : Node
{
    public ClientCallbacks Callbacks { get; private set; }
    public Control UiLayer { get; private set; }
    public Dictionary<Type, IClientComponent> Components { get; private set; }
    public ConcurrentQueue<Action> QueuedUpdates { get; }
    public Node2D GraphicsLayer { get; private set; }
    private TimerAction _uiTickTimer;
    public RefAction UiTick { get; private set; }
    public UiController UiController { get; private set; }

    public GameClient()
    {
        UiTick = new RefAction();
        _uiTickTimer = new TimerAction(.1f, 0f, UiTick.Invoke);
        Callbacks = new ClientCallbacks();
        Components = new Dictionary<Type, IClientComponent>();
        QueuedUpdates = new ConcurrentQueue<Action>();
        var canvas = new CanvasLayer();
        UiLayer = new Control();
        canvas.AddChild(UiLayer);
        UiLayer.MouseFilter = Control.MouseFilterEnum.Pass;
        UiLayer.FocusMode = Control.FocusModeEnum.None;
        AddChild(canvas);
        GraphicsLayer = new Node2D();
        AddChild(GraphicsLayer);
        var tooltip = new TooltipManager();
        AddComponent(tooltip);
        tooltip.MouseFilter = Control.MouseFilterEnum.Pass;
        tooltip.FocusMode = Control.FocusModeEnum.None;
        UiController = new UiController();
        AddComponent(UiController);
        var frame = new UiFrame();
        AddComponent(frame);
        frame.MouseFilter = Control.MouseFilterEnum.Pass;
        frame.FocusMode = Control.FocusModeEnum.None;
    }

    public override void _Input(InputEvent @event)
    {
        UiController.Mode?.HandleInput(@event);
    }

    public override void _Process(double delta)
    {
        _uiTickTimer.Process(delta);
        var values = Components.Values.ToList();
        
        foreach (var component in values)
        {
            component.Process((float)delta);
        }
        while (QueuedUpdates.TryDequeue(out var u))
        {
            u.Invoke();
        }
    }
    public Guid GetPlayerGuid()
    {
        return default;
    }
    public T GetComponent<T>() where T : class, IClientComponent
    {
        if (Components.ContainsKey(typeof(T)) == false) return null;
        return (T)Components[typeof(T)];
    }
    
    public void AddComponent(IClientComponent component)
    {
        Components.Add(component.GetType(), component);
        component.Connect(this);
    }
    private void RemoveComponent(Type type)
    {
        if (Components.ContainsKey(type) == false) return;
        var c = Components[type];
        c.Disconnect?.Invoke();
        c.Node.QueueFree();
    }
}