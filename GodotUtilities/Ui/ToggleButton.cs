using Godot;
using GodotUtilities.DataStructures.RefAction;

namespace GodotUtilities.Ui;

public partial class ToggleButton : Button
{
    private ToggleButton(Action<bool> action, ButtonGroup group)
    {
        ToggleMode = true;
        Toggled += b => action(b);
        ButtonGroup = group;
    }

    public static IEnumerable<ToggleButton> GetToggleGroup<T>(IEnumerable<T> elements,
        Action<T, bool> toggle)
    {
        var toggleAction = new RefAction();
        var buttonGroup = new ButtonGroup();
        return elements.Select(e => new ToggleButton(b => toggle(e, b), buttonGroup));
    }
}