using Godot;

namespace GodotUtilities.Ui;

public interface ISettings
{
    string Name { get; }
    List<ISettingsOption> SettingsOptions { get; }
}

public static class SettingsExt
{
    public static BoolSettingsOption MakeVisibilitySetting(
        this Node2D graphic)
    {
        var setting = new BoolSettingsOption("Visibility", 
            graphic.Visible);
        setting.SettingChanged.SubscribeForNode(v =>
        {
            graphic.Visible = v.newVal;
        }, graphic);
        return setting;
    }
    
    public static FloatSettingsOption MakeTransparencySetting(
        this Node2D graphic, string name = "Transparency")
    {
        var setting = new FloatSettingsOption(name,
            1f, .1f, 1f, .1f, false);
        setting.SettingChanged.SubscribeForNode(v =>
        {
            graphic.Modulate = new Color(1f, 1f, 1f, v.newVal);
        }, graphic);
        return setting;
    }
}