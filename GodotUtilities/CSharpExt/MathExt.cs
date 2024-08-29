using Godot;

namespace GodotUtilities.CSharpExt;

public static class MathExt
{
    public static void TestProjectToRange()
    {
        var p1 = .5f.ProjectToRange(new Vector2(0f, 1f), new Vector2(0f, 100f));
        GD.Print($"res {p1} expected {50f}");
        
        var p2 = 0f.ProjectToRange(new Vector2(-1f, 1f), new Vector2(0f, 100f));
        GD.Print($"res {p2} expected {50f}");

    }
    
    
    public static float ProjectToRange(this float v,
        Vector2 fromRange, Vector2 toRange)
    {
        if (v != Mathf.Clamp(v, fromRange.X, fromRange.Y))
        {
            throw new Exception();
        }

        var fromLength = fromRange.Y - fromRange.X;
        var prop = (v - fromRange.X) / fromLength;
        var toLength = toRange.Y - toRange.X;
        return toRange.X + prop * toLength;
    }
}