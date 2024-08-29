namespace GodotUtilities.DataStructures.Tree;

public interface INeighbored<T>
{
    IEnumerable<T> Neighbors { get; }
    void AddNeighbor(T t);
    void RemoveNeighbor(T t);
}