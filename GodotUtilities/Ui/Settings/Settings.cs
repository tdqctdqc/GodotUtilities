namespace GodotUtilities.Ui;

public class Settings : ISettings
{
    public string Name { get; private set; }

    public List<ISettingsOption> SettingsOptions { get; }

    public Settings(string name)
    {
        Name = name;
        SettingsOptions = new List<ISettingsOption>();
    }
}