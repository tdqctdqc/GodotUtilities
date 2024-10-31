using Godot;
using GodotUtilities.DataStructures.Hex;

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

    public static void DrawHex(this MeshBuilder mb,
        Vector2 center, float radius, Color color)
    {
        var ne = (Vector2.Up * radius).Rotated(Mathf.Pi / 6f);
        var e = ne.Rotated(Mathf.Pi / 3f);
        var se = e.Rotated(Mathf.Pi / 3f);
        var sw = se.Rotated(Mathf.Pi / 3f);
        var w = sw.Rotated(Mathf.Pi / 3f);
        var nw = w.Rotated(Mathf.Pi / 3f);
        
        mb.AddTri(center, center + ne, center + e, color);
        mb.AddTri(center, center + e, center + se, color);
        mb.AddTri(center, center + se, center + sw, color);
        mb.AddTri(center, center + sw, center + w, color);
        mb.AddTri(center, center + w, center + nw, color);
        mb.AddTri(center, center + nw, center + ne, color);
    }

    public static void DrawHexBorder(this MeshBuilder mb, Vector3I hex, Vector3I nHex,
        Color color)
    {
        var angle = hex.GetHexAngle(nHex);
        var pos = hex.GetWorldPos();
        
        
        
        for (var i = 0; i < HexExt.HexBorderShape.Length; i+=3)
        {
            var a = HexExt.HexBorderShape[i].Rotated(angle) + pos;
            var b = HexExt.HexBorderShape[i + 1].Rotated(angle) + pos;
            var c = HexExt.HexBorderShape[i + 2].Rotated(angle) + pos;
            mb.AddTri(a,b,c,color);
        }
    }
}