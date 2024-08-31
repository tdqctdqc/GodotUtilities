
namespace GodotUtilities.DataStructures.Picker;

public interface IPickerAgent<T>
{
    IEnumerable<T> Pick(Picker<T> host);
    HashSet<T> Seeds { get; }
    HashSet<T> Picked { get; }
}