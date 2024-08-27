
namespace GodotUtilities.DataStructures;

public interface IReadOnlyHash<T> : IReadOnlyCollection<T>
{
    bool Contains(T t);
}