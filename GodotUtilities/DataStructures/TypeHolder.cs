namespace GodotUtilities.DataStructures;

public class TypeHolder<TAttr> 
{
    public Dictionary<Type, TAttr> Elements { get; private set; }
    public TAttr this[Type type] => Elements.ContainsKey(type) ? Elements[type] : default;

    public TypeHolder()
    {
        Elements = new Dictionary<Type, TAttr>();
    }
    public void Add(TAttr t)
    {
        Elements.Add(t.GetType(), t);
    }

    public bool Has<T>()
    {
        return Elements.ContainsKey(typeof(T));
    }
    public T Get<T>() where T : TAttr
    {
        return (T) Elements[typeof(T)];
    }
}