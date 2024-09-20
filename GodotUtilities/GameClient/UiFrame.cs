using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotUtilities.GameClient;
using GodotUtilities.Ui;

public partial class UiFrame : VBoxContainer, 
    IClientComponent
{
    public VBoxContainer TopBars { get; private set; }
    public MultiBar LeftBar { get; private set; }
    public VBoxContainer RightSidebar { get; private set; }
    public Action Disconnect { get; set; }

    public void Connect(GameClient client)
    {
        this.FullRect();
        TopBars = new VBoxContainer();
        AddChild(TopBars);
        var sidebars = new HBoxContainer();
        AddChild(sidebars);

        sidebars.SetAnchorsPreset(LayoutPreset.HcenterWide);
        sidebars.AnchorsPreset = (int)(LayoutPreset.HcenterWide);
            
        LeftBar = MultiBar.MakeVertical();
        LeftBar.SetAnchorsPreset(LayoutPreset.LeftWide);
        sidebars.AddChild(LeftBar);
        var filler = new Control();
        filler.GrowHorizontal = GrowDirection.Both;
        filler.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        sidebars.AddChild(filler);
        
        
        RightSidebar = new VBoxContainer();
        RightSidebar.SetAnchorsPreset(LayoutPreset.RightWide);
        sidebars.AddChild(RightSidebar);
        client.UiLayer.AddChild(this);
    }

    public void Process(float delta)
    {
        
    }

    public override void _Ready()
    {
        CustomMinimumSize = GetViewportRect().Size;
    }

    public void AddTopBar(Node topBar)
    {
        TopBars.AddChild(topBar);
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        GD.Print("ui frame getting unhandled input");
    }
    public override void _Input(InputEvent @event)
    {
        GD.Print("ui frame getting input");
    }
    Node IClientComponent.Node => this;
}
