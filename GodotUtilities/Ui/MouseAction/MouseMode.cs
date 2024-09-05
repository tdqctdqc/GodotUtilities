using Godot;

namespace GodotUtilities.Ui;

public class MouseMode
{
    public List<MouseAction> Actions { get; private set; }
    public MouseMode(List<MouseAction> actions)
    {
        Actions = actions;
    }

    public void HandleInput(InputEventMouse m)
    {
        foreach (var action in Actions)
        {
            action.Process(m);
        }
    }
}