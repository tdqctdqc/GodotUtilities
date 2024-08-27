using Godot;

namespace GodotUtilities.DataStructures.ShapingFunctions;

public class ArctanFunction : IFunction<float, float>
{
    private float _squarenessFactor;
    private static float _tanHalf = Mathf.Tan(.5f);

    public ArctanFunction(float halfwayX)
    {
        _squarenessFactor = _tanHalf / halfwayX;
    }

    public float Calc(float t)
    {
        return Mathf.Atan(t * _squarenessFactor) / (Mathf.Pi / 2f);
    }
}