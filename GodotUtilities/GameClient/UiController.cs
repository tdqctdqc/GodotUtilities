
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace GodotUtilities.GameClient;
using GodotUtilities.Ui;

public partial class UiController : Node, IClientComponent
{
    public UiMode Mode => ModeOption.Value;
    public ListSettingsOption<UiMode> ModeOption { get; private set; }
    public Node Node => this;
    public Action Disconnect { get; set; }

    public void Connect(GameClient client)
    {
        Disconnect += () => throw new Exception();
        var modes = new List<UiMode>
        {
            
        };
        var names = modes.Select(m => m.GetType().Name).ToList();
        ModeOption = new ListSettingsOption<UiMode>("Ui Mode",
            modes, names);
        ModeOption.SettingChanged.Subscribe(v =>
        {
            v.oldVal?.Clear();
            v.newVal.Enter();
            client.GetComponent<UiFrame>().LeftBar.SetLabel(v.newVal.Name);
        });
    }
    public override void _UnhandledInput(InputEvent e)
    {
        GD.Print("ui controller getting unhandled input");
    }
    public override void _Input(InputEvent e)
    {
        GD.Print("ui controller getting input");
    }
    public void Process(float delta)
    {
        Mode?.Process(delta);
    }
}