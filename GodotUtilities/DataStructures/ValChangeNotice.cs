namespace GodotUtilities.DataStructures;

public class ValChangeNotice<TOwner, TProperty> 
{
    public TOwner Owner { get; private set; }
    public TProperty NewVal { get; private set; }
    public TProperty OldVal { get; private set; }

    public ValChangeNotice(TOwner owner, TProperty newVal, TProperty oldVal)
    {
        Owner = owner;
        NewVal = newVal;
        OldVal = oldVal;
    }
    public void Clear()
    {
        NewVal = default;
        OldVal = default;
    }
}