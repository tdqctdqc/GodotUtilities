using GodotUtilities.DataStructures.AggregateTree;

namespace GodotUtilities.DataStructures.Tree;

public static class BranchExt
{
    public static IEnumerable<Branch<T>> GetAtDepth<T>(
        this IEnumerable<Branch<T>> top, int depth)
            where T : class, IIded
    {
        if (depth == 0) return top;
        return top.SelectMany(t => t.Children.GetAtDepth(depth - 1));
    }
}