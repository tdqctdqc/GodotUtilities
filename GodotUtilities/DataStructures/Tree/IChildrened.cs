namespace GodotUtilities.DataStructures.Tree;

public interface IChildrened<T>
{
    IEnumerable<T> Children { get; }
    void AddChild(T t);
    void RemoveChild(T t);
}