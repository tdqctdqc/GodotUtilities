using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Godot;
namespace GodotUtilities.GameClient;

public partial class TooltipManager : Control, IClientComponent
{
    private TooltipPanel _panel;
    private object _element;
    private Vector2 _offsetFromMouse = new Vector2(20f, 20f);
    
    Node IClientComponent.Node => this;
    public void Connect(GameClient client)
    {
        _panel = new TooltipPanel();
        AddChild(_panel);
        _panel.Visible = false;
        client.UiLayer.AddChild(this);
    }

    public override void _UnhandledInput(InputEvent e)
    {
        GD.Print($"{GetType().Name} getting unhandled input");
    }
    public override void _Input(InputEvent e)
    {
        GD.Print($"{GetType().Name} getting input");
    }
    public Action Disconnect { get; set; }

    public void Clear()
    {
        _panel.Visible = false;
    }
    public void Process(float delta)
    {
        if(_element != null) _panel.Move(GetLocalMousePosition() + _offsetFromMouse);
    }
    public void Prompt<TElement>
        (TooltipTemplate<TElement> template, TElement element)
    {
        _panel.Visible = true;
        _element = element;
        _panel.Setup(template.GetFastContainer(element),
            template.GetSlowContainer(element), 
            element);
    }
    public void Prompt<TElement>
        (VBoxContainer fast, VBoxContainer slow, TElement element)
    {
        _panel.Visible = true;
        _element = element;
        _panel.Setup(fast, slow, element);
    }

    public void HideTooltip(object element)
    {
        if (_element == element)
        {
            _panel.Visible = false;
            _element = null;
        }
    }
}