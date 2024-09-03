using Godot;

namespace GodotUtilities.Graphics;

public class HexColorMesh<TElement> : MeshInstance2D
{
    private Vector2[] _vertices;
    private IReadOnlyList<TElement> _elements;
    private Color[] _colors;
    private ArrayMesh _arrayMesh;
    public HexColorMesh(
        IReadOnlyList<TElement> elements,
        Func<TElement, Vector2> getCenter,
        float radius)
    {
        _vertices = ShapeBuilder.GetHexes(elements.Select(getCenter), radius)
            .ToArray();
        _elements = elements;
        _colors = new Color[elements.Count * 6 * 3];
    }

    public void SetColors(Func<TElement, Color> getColor)
    {
        int iter = 0;
        for (var i = 0; i < _elements.Count; i++)
        {
            var e = _elements[i];
            var color = getColor(e);
            var triCount = 6;
            for (var j = 0; j < triCount; j++)
            {
                _colors[iter] = color;
                iter++;
                _colors[iter] = color;
                iter++;
                _colors[iter] = color;
                iter++;
            }
        }        
        if (_vertices.Length < 3) _arrayMesh = new ArrayMesh();
        else _arrayMesh = MeshGenerator.GetArrayMesh(_vertices,
            null,
            _colors);
        Mesh = _arrayMesh;
    }
}