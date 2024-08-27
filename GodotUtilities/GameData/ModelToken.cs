namespace GodotUtilities.GameData;

public class ModelToken<T> : Entity
    where T : Model
{
    public string Name { get; private set; }

    public ModelToken(int id, string name) : base(id)
    {
        Name = name;
    }

    public T Get(Data d)
    {
        return (T)d.Models.ModelsByName[Name];
    }
    public override void Made(Data d)
    {
    }

    public override void CleanUp(Data d)
    {
    }
}