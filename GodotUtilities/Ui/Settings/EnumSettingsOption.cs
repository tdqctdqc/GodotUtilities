namespace GodotUtilities.Ui;

public class EnumSettingsOption<T> : ListSettingsOption<T>
    where T : struct, System.Enum
{
    public EnumSettingsOption(string name) 
        : base(name, 
            System.Enum.GetValues<T>().ToList(),
            System.Enum.GetNames<T>().ToList())
    {
        Set(System.Enum.GetValues<T>()[0]);
    }
}