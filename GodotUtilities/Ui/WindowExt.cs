using Godot;

namespace GodotUtilities.Ui;

public static class WindowExt
{
    public static void MakeHideable(this Window w)
    {
        w.CloseRequested += w.Hide;
        w.WindowInput += i =>
        {
            if (i is InputEventKey k && k.Keycode == Godot.Key.Escape)
            {
                w.Hide();
                w.GetViewport().SetInputAsHandled();
            }
        };
    }

    public static void MakeFreeable(this Window w)
    {
        w.CloseRequested += w.QueueFree;
        w.WindowInput += i =>
        {
            if (i is InputEventKey k && k.Keycode == Godot.Key.Escape)
            {
                w.QueueFree();
                w.GetViewport().SetInputAsHandled();
            }
        };
    }

    public static T MakeScroll<T>(this Window w)
        where T : Container, new()
    {
        var panel = new Panel();
        panel.FullRect();
        panel.ExpandFill();
        var t = panel.MakeScrollChild<T>(out var scroll);
        w.AddChild(panel);
        return t;
    }
}