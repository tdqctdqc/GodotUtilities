using Godot;

namespace GodotUtilities.Graphics;

public partial class HexColorMultiMesh<TElement> 
    : MultiMeshInstance2D
{
    public Dictionary<TElement, int> Indices { get; private set; }
    public HexColorMultiMesh(
        IReadOnlyList<TElement> elements,
        Func<TElement, Vector2> getCenter,
        Func<TElement, Color> getColor,
        float radius)
    {
        Indices = new Dictionary<TElement, int>();
        Multimesh = new MultiMesh();
        Multimesh.UseColors = true;
        Multimesh.InstanceCount = elements.Count;
        var hexVerts = ShapeBuilder.GetHex(Vector2.Zero, radius);
        var mesh = MeshGenerator.GetArrayMesh(hexVerts.ToArray());
        Multimesh.Mesh = mesh;
        int iter = 0;
        foreach (var element in elements)
        {
            var color = getColor(element);
            var center = getCenter(element);
            Multimesh.SetInstanceColor(iter, color);
            Multimesh.SetInstanceTransform2D(iter, new Transform2D(0f, center));
            Indices.Add(element, iter);
            iter++;
        }
    }

    public void SetColor(TElement element, Color color)
    {
        var index = Indices[element];
        Multimesh.SetInstanceColor(index, color);
    }

    public void SetColors(Func<TElement, Color> getColor)
    {
        foreach (var (element, index) in Indices)
        {
            Multimesh.SetInstanceColor(index, getColor(element));
        }
    }
}