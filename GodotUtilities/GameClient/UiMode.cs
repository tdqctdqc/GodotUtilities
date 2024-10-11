
using Godot;
using GodotUtilities.Ui;

namespace GodotUtilities.GameClient;

public abstract class UiMode
{
    protected GameClient _client;
    public string Name { get; private set; }
    public DefaultSettingsOption<MouseMode> MouseMode { get; private set; }
    protected UiMode(GameClient client, string name)
    {
        Name = name;
        MouseMode = new DefaultSettingsOption<MouseMode>("Mouse Mode", null);
        _client = client;
    }

    public abstract void Process(float delta);
    public abstract void HandleInput(InputEvent e);

    public abstract void Enter();
    public abstract void Clear();

    public virtual void Tooltip()
    {
        _client.GetComponent<TooltipManager>()
            .Clear();
    }

    public abstract Control GetControl(GameClient client);
}