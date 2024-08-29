using Godot;

namespace GodotUtilities.DataStructures.Picker;

public static class PickerFuncs
{
    public static Func<T, HashSet<T>, HashSet<T>> ChooseRandom<T>(
        int leafSize, Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, bool> canShare, RandomNumberGenerator random)
    {
        return (t, free) => FloodFill<T>.FloodFillRandomishToLimit(
            t, leafSize, getNeighbors,
            n => free.Contains(n) && canShare(t, n),
            random);
    }
}