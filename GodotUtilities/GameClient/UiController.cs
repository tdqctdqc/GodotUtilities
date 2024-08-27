
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace GodotUtilities.GameClient;
using GodotUtilities.Ui;

public partial class UiController : Node, IClientComponent
{
    private GameClient _client;
    public UiMode Mode => ModeOption.Value;
    public ListSettingsOption<UiMode> ModeOption { get; private set; }
    public Node Node => this;

    public UiController(GameClient client)
    {
        _client = client;
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
            _client.GetComponent<UiFrame>().LeftBar.SetLabel(v.newVal.Name);
        });
    }


    public Action Disconnect { get; set; }
    public void Process(float delta)
    {
        Mode?.Process(delta);
    }
}