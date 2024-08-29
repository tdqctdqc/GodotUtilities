using Godot;
using Poly2Tri.Triangulation.Delaunay.Sweep;
using Poly2Tri.Triangulation.Polygon;

namespace GodotUtilities.Graphics;

public class MeshBuilder
{
    public List<Vector2> TriVertices { get; private set; }
    public List<Color> Colors { get; private set; }
    public List<Label> Labels { get; private set; }
    

    

    public MeshBuilder()
    {
        TriVertices = new List<Vector2>();
        Colors = new List<Color>();
        Labels = new List<Label>();
    }
    
    public void Clear()
    {
        TriVertices.Clear();
        Colors.Clear();
    }

   
    public void AddTri(Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        TriVertices.Add(a);
        TriVertices.Add(b);
        TriVertices.Add(c);
        Colors.Add(color);
        Colors.Add(color);
        Colors.Add(color);
    }
    public void AddTris(IEnumerable<Vector2> points, Color color)
    {
        var count = points.Count();
        for (var i = 0; i < count; i+=3)
        {
            TriVertices.Add(points.ElementAt(i));
            TriVertices.Add(points.ElementAt(i+1));
            TriVertices.Add(points.ElementAt(i+2));
            
            Colors.Add(color);
            Colors.Add(color);
            Colors.Add(color);
        }
    }
    
    
    private void JoinLinePoints(Vector2 from, Vector2 to, float thickness, Color color)
    {
        var perpendicular = (from - to).Normalized().Rotated(Mathf.Pi / 2f);
        var fromOut = from + perpendicular * .5f * thickness;
        var fromIn = from - perpendicular * .5f * thickness;
        var toOut = to + perpendicular * .5f * thickness;
        var toIn = to - perpendicular * .5f * thickness;
        AddTri(fromIn, fromOut, toOut, color);
        AddTri(toIn, toOut, fromIn, color);
    }

    
    public void DrawBorderInset(Color innerColor, Color borderColor, float borderThickness, float insetThickness,
        Vector2[] boundary)
    {
        var insets = Geometry2D.OffsetPolygon(
            boundary,
            -insetThickness);
        foreach (var inset in insets)
        {
            var poly = new Poly2Tri.Triangulation.Polygon.Polygon(
                inset.Select(v => new PolygonPoint(v.X, v.Y)));
            var ctx = new DTSweepContext();

            var inners = Geometry2D.OffsetPolygon(
                inset, -borderThickness);
            foreach (var inner in inners)
            {
                this.DrawPolygon(inner, innerColor);
                var hole = new Poly2Tri.Triangulation.Polygon.Polygon(
                    inner.Select(v => new PolygonPoint(v.X, v.Y)));
                poly.AddHole(hole);
            }

            Poly2Tri.P2T.Triangulate(poly.Yield());

            foreach (var tri in poly.Triangles)
            {
                var a = new Vector2((float)tri.Points[0].X, (float)tri.Points[0].Y);
                var b = new Vector2((float)tri.Points[1].X, (float)tri.Points[1].Y);
                var c = new Vector2((float)tri.Points[2].X, (float)tri.Points[2].Y);
                AddTri(a, b, c, borderColor);
            }
        }
    }


    public void AddLine(Vector2 from, Vector2 to, Color color, float thickness)
    {
        JoinLinePoints(from, to, thickness, color);
    }

    public void AddParallelLines(Vector2 from, Vector2 to, Color color, float thickness, float offset)
    {
        var axis = (to - from).Normalized();
        var perp = axis.Orthogonal();
        var railOffset = offset * perp;
        var railWidth = thickness;
        AddLine(from + railOffset, to + railOffset, color, railWidth);
        AddLine(from - railOffset, to - railOffset, color, railWidth);
    }

    public void AddSpacedCrossbars(Vector2 from, Vector2 to, Color color, float thickness, float length, float spacing)
    {
        var axis = (to - from).Normalized();
        var perp = axis.Orthogonal();
        var numCrossBars = Mathf.FloorToInt(from.DistanceTo(to) / spacing);
        var crossStartOffset = axis * spacing / 2f;
        for (var i = 0; i < numCrossBars; i++)
        {
            var mid = crossStartOffset + axis * i * spacing;
            var left = from + mid - perp * length;
            var right = from + mid + perp * length;
            AddLine(left, right, color, thickness);
        }
    }
    
    public void AddDashedLine(Vector2 from, Vector2 to, Color color, float thickness, float dashLength, float spacing)
    {
        var axis = (to - from).Normalized();
        var perp = axis.Orthogonal();
        var numCrossBars = Mathf.FloorToInt(from.DistanceTo(to) / (spacing + dashLength));
        var startOffset = axis * spacing / 2f;
        for (var i = 0; i < numCrossBars; i++)
        {
            var dashFrom = from + startOffset + axis * i * (spacing + dashLength);
            var dashTo = dashFrom + axis * dashLength;
            AddLine(dashFrom, dashTo, color, thickness);
        }
    }
    public void AddLines(List<Vector2> froms,
        List<Vector2> tos, float thickness, List<Color> colors)
    {
        for (int i = 0; i < froms.Count; i++)
        {
            var color = colors[i];
            JoinLinePoints(froms[i], tos[i], thickness, color);
        }
    }
    
    public void AddLinesCustomWidths(List<Vector2> froms,
        List<Vector2> tos, List<float> widths, List<Color> colors)
    {
        for (int i = 0; i < froms.Count; i++)
        {
            var color = colors[i];
            JoinLinePoints(froms[i], tos[i], widths[i], color);
        }
    }
    
    public void AddCircle(Vector2 center, float radius, int resolution, Color color)
    {
        var angleIncrement = Mathf.Pi * 2f / (float) resolution;
        var triPoints = new List<Vector2>();
        for (int i = 0; i < resolution; i++)
        {
            var startAngle = angleIncrement * i;
            var startPoint = center + Vector2.Up.Rotated(startAngle) * radius;
            var endAngle = startAngle + angleIncrement;
            var endPoint = center + Vector2.Up.Rotated(endAngle) * radius;
            AddTri(center, startPoint, endPoint, color);
        }
    }

    public void AddArrow(Vector2 from, Vector2 to, 
        float thickness, Color color)
    {
        var length = from.DistanceTo(to);
        var arrowLength = Mathf.Min(length / 2f, thickness * 1.5f);
        var stemLength = length - arrowLength;

        var axis = (to - from).Normalized();
        var orth = axis.Orthogonal();

        var arrowBase = from + axis * stemLength;
        
        AddTri(to, arrowBase + orth * thickness,
            arrowBase - orth * thickness, color);
        AddLine(from, arrowBase, color, thickness);
    }
    

    public void AddNumMarkers(List<Vector2> points, float markerSize, Color color, Color textColor, Vector2 offset,
        string tag = "")
    {
        AddPointMarkers(points, markerSize, color);
        for (var i = 0; i < points.Count; i++)
        {
            var label = new Label();
            label.Text = tag + " " + i.ToString();
            label.Position = points[i] + offset;
            label.SelfModulate = textColor;
            Labels.Add(label);
        }
    }
    public void AddPointMarkers(List<Vector2> points, float markerSize, Color color)
    {
        foreach (var p in points)
        {
            AddSquare(p, markerSize, color);
        }
    }

    public void AddSquare(Vector2 p, float size, Color color)
    {
        var topLeft = p + Vector2.Up * size / 2f
                        + Vector2.Left * size / 2f;
        var topRight = p + Vector2.Up * size / 2f
                         + Vector2.Right * size / 2f;
        var bottomLeft = p + Vector2.Down * size / 2f
                           + Vector2.Left * size / 2f;
        var bottomRight = p + Vector2.Down * size / 2f
                            + Vector2.Right * size / 2f;
        AddTri(topLeft, topRight, bottomLeft, color);
        AddTri(topRight, bottomRight, bottomLeft, color);
    }
    public MeshInstance2D GetMeshInstance()
    {
        if (TriVertices.Count == 0) return new MeshInstance2D();
        var mesh = MeshGenerator.GetArrayMesh(
            TriVertices.ToArray(), 
            Colors.ToArray());
        var meshInstance = new MeshInstance2D();
        meshInstance.Mesh = mesh;
        Labels.ForEach(l => meshInstance.AddChild(l));
        return meshInstance;
    }

    public Mesh GetMesh()
    {
        if (TriVertices.Count == 0) return null;
        var mesh = MeshGenerator.GetArrayMesh(
            TriVertices.ToArray(), 
            Colors.ToArray());
        return mesh;
    }
}