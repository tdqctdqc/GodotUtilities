namespace GodotUtilities.Ui;

public class MultiSettings
{
    public string Name { get; private set; }
    public List<ISettings> Settings { get; private set; }
    public MultiSettings(string name, List<ISettings> settings)
    {
        Name = name;
        Settings = settings;
    }
}