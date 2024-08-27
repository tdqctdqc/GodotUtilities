using Godot;
using GodotUtilities.DataStructures.RefAction;

namespace GodotUtilities.Ui;

public abstract class SettingsOption<T> : ISettingsOption
{
    public string Name { get; private set; }
    public T Value { get; private set; }
    public RefAction<(T oldVal, T newVal)> SettingChanged { get; private set; }
    RefAction ISettingsOption.SettingChanged => SettingChanged.Blank;
    public abstract Control GetControlInterface();
    
    protected SettingsOption(string name, T value)
    {
        Name = name;
        Value = value;
        SettingChanged = new RefAction<(T, T)>();
    }

    public void Set(T val)
    {
        // if (Value is not null && Value.Equals(val)) return;
        // if (Value is null && val is null) return;
        var oldVal = Value;
        Value = val;
        SettingChanged.Invoke((oldVal, val));
    }
}