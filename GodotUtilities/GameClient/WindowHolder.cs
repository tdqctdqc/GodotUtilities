using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using GodotUtilities.GameClient;

public partial class WindowHolder : Node
{
    public Action Disconnect { get; set; }
    public void Process(float delta)
    {
        
    }
    public WindowHolder(GameClient client)
    {
       client.UiLayer.AddChild(this);
    }
    public void OpenWindowFullSize(Window w)
    {
        AddChild(w);
        w.Size = DisplayServer.WindowGetSize();
        w.PopupCenteredClamped(null, .9f);
    }
    public void OpenWindow(Window w)
    {
        AddChild(w);
        w.PopupCenteredClamped(w.Size);
    }
}
