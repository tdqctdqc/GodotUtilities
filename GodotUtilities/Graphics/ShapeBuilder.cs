
using System.Collections;
using Godot;
using GodotUtilities.DataStructures.Hex;

public static class ShapeBuilder
{
    public static IEnumerable<Vector2> GetArrow(Vector2 from, Vector2 to,
        float thickness)
    {
        var length = from.DistanceTo(to);
        var arrowLength = Mathf.Min(length / 2f, thickness * 1.5f);
        var stemLength = length - arrowLength;

        var axis = (to - from).Normalized();
        var orth = axis.Orthogonal();
        var arrowBase = from + axis * stemLength;

        
        yield return from + orth * thickness;
        yield return from - orth * thickness;
        yield return arrowBase + orth * thickness;
        
        yield return arrowBase - orth * thickness;
        yield return arrowBase + orth * thickness;
        yield return from - orth * thickness;

        yield return to;
        yield return arrowBase - orth * thickness * 2f;
        yield return arrowBase + orth * thickness * 2f;
    }

    public static IEnumerable<Vector2> GetHexes(IEnumerable<Vector2> centers, float radius)
    {
        return centers.SelectMany(c => GetHex(c, radius));
    }

    public static IEnumerable<Vector2> GetHexBorder(Vector3I coords, 
        Vector3I dir, float radius, float thickness)
    {
        var angle = coords.GetHexAngle(coords + dir);
        var worldPos = coords.GetWorldPos();
        var nWorldPos = (dir + coords).GetWorldPos();
        var worldDir = (nWorldPos - worldPos).Normalized() * radius;
        
        var thicknessHypotenuse = thickness * Mathf.Sqrt(3) / 2f;
        
        var r1 = worldDir.Rotated(Mathf.Pi / 6f);
        var r2 = worldDir.Rotated(-Mathf.Pi / 6f);
        var p1 = worldPos + r1;
        var p2 = worldPos + r2;
        var p3 = p1 + 
                 thicknessHypotenuse * worldDir.Rotated(5f * Mathf.Pi / 6f);
        var p4 = p2 + 
                 thicknessHypotenuse * worldDir.Rotated(-5f * Mathf.Pi / 6f);
        yield return p1;
        yield return p2;
        yield return p3;
        
        yield return p3;
        yield return p2;
        yield return p4;
    }
    public static IEnumerable<Vector2> GetHex(Vector2 center, float radius)
    {
        var ne = center + radius * new Vector2(.5f, -.866f);
        var e = center + radius * new Vector2(1f, 0f);
        var se = center + radius * new Vector2(.5f, .866f);
        var sw = center + radius * new Vector2(-.5f, .866f);
        var w = center + radius * new Vector2(-1f, 0f);
        var nw = center + radius * new Vector2(-.5f, -.866f);

        yield return nw;
        yield return ne;
        yield return center;
        
        yield return ne;
        yield return e;
        yield return center;
        
        yield return e;
        yield return se;
        yield return center;
        
        yield return se;
        yield return sw;
        yield return center;
        
        yield return sw;
        yield return w;
        yield return center;
        
        yield return w;
        yield return nw;
        yield return center;
    }
}