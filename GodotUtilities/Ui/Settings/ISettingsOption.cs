using Godot;
using GodotUtilities.DataStructures.RefAction;

namespace GodotUtilities.Ui;

public interface ISettingsOption
{
    string Name { get; }
    RefAction SettingChanged { get; }
    Control GetControlInterface();
}