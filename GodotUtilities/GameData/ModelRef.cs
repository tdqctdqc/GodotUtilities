namespace GodotUtilities.GameData;

public readonly struct ModelRef<T>(string name) : IRef<T>
    where T : Model
{
    public string Name { get; } = name;

    public T Get(Data d)
    {
        return (T)d.Models.ModelsByName[Name];
    }
}