namespace GodotUtilities.GameData;

public class ModelRef<T> : IRef<T> where T : Model
{
    public string Name { get; private set; }

    public ModelRef(string name)
    {
        Name = name;
    }

    public T Get(Data d)
    {
        return (T)d.Models.ModelsByName[Name];
    }
}