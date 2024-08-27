using GodotUtilities.DataStructures;
namespace GodotUtilities.GameData;

public interface IRef<out T>
{
    T Get(Data d);
}

public interface IIdRef<out T> : IRef<T>
{
    int Id { get; }
}

public static class IdRefExt
{
    public static bool IsEmpty<T>(this IIdRef<T> i)
        where T : IIded
    {
        return i.Id == -1;
    }

    public static bool Fulfilled<T>(this IIdRef<T> i)
        where T : IIded
    {
        return i.Id != -1;
    }
}