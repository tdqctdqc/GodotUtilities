using Godot;

namespace GodotUtilities.Ui;

public interface IMouseAction
{
    void Process(InputEventMouse m);
}