namespace GodotUtilities.Ui;

public interface ISettings
{
    string Name { get; }
    List<ISettingsOption> SettingsOptions { get; }
}