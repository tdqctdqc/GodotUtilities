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
        MouseFilter = MouseFilterEnum.Ignore;
        this.FullRect();
        
        TopBars = new VBoxContainer();
        TopBars.MouseFilter = MouseFilterEnum.Stop;
        AddChild(TopBars);
        var sidebars = new HBoxContainer();
        sidebars.MouseFilter = MouseFilterEnum.Ignore;
        AddChild(sidebars);

        sidebars.SetAnchorsPreset(LayoutPreset.HcenterWide);
        sidebars.AnchorsPreset = (int)(LayoutPreset.HcenterWide);
            
        LeftBar = MultiBar.MakeVertical();
        LeftBar.SetAnchorsPreset(LayoutPreset.LeftWide);
        LeftBar.MouseFilter = MouseFilterEnum.Stop;
        sidebars.AddChild(LeftBar);
        var filler = new Control();
        filler.GrowHorizontal = GrowDirection.Both;
        filler.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        filler.MouseFilter = MouseFilterEnum.Ignore;
        sidebars.AddChild(filler);
        
        
        RightSidebar = new VBoxContainer();
        RightSidebar.SetAnchorsPreset(LayoutPreset.RightWide);
        RightSidebar.MouseFilter = MouseFilterEnum.Stop;
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
    Node IClientComponent.Node => this;
}
