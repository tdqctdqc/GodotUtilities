using Godot;

namespace GodotUtilities.Graphics;

public static class MeshBuilderExt
{
    public static void DrawBox(this MeshBuilder mb,
        Vector2 topLeft, Vector2 bottomRight, Color color,
        float thickness)
    {
        var topRight = new Vector2(bottomRight.X, topLeft.Y);
        var bottomLeft = new Vector2(topLeft.X, bottomRight.Y);
        mb.AddLine(topLeft, topRight, color, thickness);
        mb.AddLine(topLeft, bottomLeft, color, thickness);
        mb.AddLine(topRight, bottomRight, color, thickness);
        mb.AddLine(bottomLeft, bottomRight, color, thickness);
    }
    
    public static void DrawPolygonOutline(this MeshBuilder mb,
        Vector2[] boundaryPoints, float thickness, Color color)
    {
        for (var i = 0; i < boundaryPoints.Length; i++)
        {
            var from = boundaryPoints[i];
            var to = boundaryPoints.Modulo(i + 1);
            mb.AddLine(from, to, color, thickness);
        }
    }
    public static void DrawPolygon(this MeshBuilder mb,
        Vector2[] boundaryPoints, Color color)
    {
        var tris = Geometry2D.TriangulatePolygon(boundaryPoints);
        for (var i = 0; i < tris.Length; i+=3)
        {
            var p1 = boundaryPoints[tris[i]];
            var p2 = boundaryPoints[tris[i+1]];
            var p3 = boundaryPoints[tris[i+2]];
            mb.AddTri(p1, p2, p3, color);
        }
    }
}