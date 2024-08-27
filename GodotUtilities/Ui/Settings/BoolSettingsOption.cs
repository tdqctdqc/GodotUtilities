using Godot;

namespace GodotUtilities.Ui;

public class BoolSettingsOption : SettingsOption<bool>
{
    public BoolSettingsOption(string name, bool value) : base(name, value)
    {
    }

    public override Control GetControlInterface()
    {
        var hbox = new HBoxContainer();
        var label = new Label();
        hbox.AddChild(label);
        label.Text = $"{Name}: ";
        var check = new CheckBox();
        check.FocusMode = Control.FocusModeEnum.None;
        check.ToggleMode = true;
        check.ButtonPressed = Value;
        check.Toggled += Set;
        SettingChanged.Subscribe(b => check.ButtonPressed = b.newVal);
        hbox.AddChild(check);
        hbox.CustomMinimumSize = check.Size;
        return hbox;
    }
}