namespace GodotUtilities.GameData;

public class ModelComponentHolder
{


    public List<IModelComponent> Components { get; private set; }
        = new List<IModelComponent>();

    public IEnumerable<T> OfType<T>()
    {
        return Components.OfType<T>();
    }

    public T Get<T>()
    {
        return Components.OfType<T>().FirstOrDefault();
    }
    public void Add(IModelComponent c)
    {
        Components.Add(c);
    }
}