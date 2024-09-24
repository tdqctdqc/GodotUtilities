using System.Collections.Concurrent;
using Godot;
using GodotUtilities.DataStructures.RefAction;
using GodotUtilities.GameData;
using GodotUtilities.Logic;
using GodotUtilities.Server;

namespace GodotUtilities.GameClient;

public class GameClient : Node
{
    public Guid PlayerGuid { get; private set; }
    protected ILogic _logic;
    public ClientCallbacks Callbacks { get; private set; }
    public Control UiLayer { get; private set; }
    public Dictionary<Type, IClientComponent> Components { get; private set; }
    public ConcurrentQueue<Action> QueuedUpdates { get; }
    public Node2D GraphicsLayer { get; private set; }
    private TimerAction _uiTickTimer;
    public RefAction UiTick { get; private set; }
    public UiController UiController { get; private set; }
    public WindowHolder WindowHolder { get; private set; }
    public GameClient(ILogic logic, Guid playerGuid)
    {
        _logic = logic;
        _logic.MessageForLocalClient += HandleMessageForClient;
        PlayerGuid = playerGuid;
        UiTick = new RefAction();
        _uiTickTimer = new TimerAction(.1f, 0f, UiTick.Invoke);
        Callbacks = new ClientCallbacks();
        Components = new Dictionary<Type, IClientComponent>();
        QueuedUpdates = new ConcurrentQueue<Action>();
        var canvas = new CanvasLayer();
        canvas.Layer = 100;
        UiLayer = new Control();
        canvas.AddChild(UiLayer);
        UiLayer.FocusMode = Control.FocusModeEnum.None;
        WindowHolder = new WindowHolder(this);
        
        AddChild(canvas);
        GraphicsLayer = new Node2D();
        AddChild(GraphicsLayer);
        var tooltip = new TooltipManager();
        AddComponent(tooltip);
        tooltip.FocusMode = Control.FocusModeEnum.None;
        UiController = new UiController();
        AddComponent(UiController);
        var frame = new UiFrame();
        AddComponent(frame);
        frame.FocusMode = Control.FocusModeEnum.None;
    }

    public void SubmitCommand(Command c)
    {
        c.SetCommandingPlayer(PlayerGuid);
        _logic.HandleMessageFromClient(c);
    }
    public override void _Process(double delta)
    {
        _logic.Process(delta);
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

    private void HandleMessageForClient(Message m)
    {
        if (m is ClientMessage cm)
        {
            cm.Handle(this);
        }
        else throw new Exception($"client couldnt handle message of type {m.GetType()}");
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