using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotUtilities.Graphics;

public abstract partial class TriColorMesh<TElement> 
    : MeshInstance2D
{
    private IReadOnlyList<int> _elementTriCounts;
    private Vector2[] _vertices;
    private IReadOnlyList<TElement> _elements;
    private Color[] _colors;
    private ArrayMesh _arrayMesh;

    public TriColorMesh(
        IReadOnlyList<int> elementTriCounts,
        IReadOnlyList<TElement> elements,
        Vector2[] vertices)
    {
        _vertices = vertices;
        _elementTriCounts = elementTriCounts;
        _elements = elements;
        _colors = new Color[_elementTriCounts.Sum() * 3];
    }

    public void Draw(Func<TElement, Color> getColor)
    {
        int iter = 0;
        for (var i = 0; i < _elements.Count; i++)
        {
            var e = _elements[i];
            var color = getColor(e);
            var triCount = _elementTriCounts[i];
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
