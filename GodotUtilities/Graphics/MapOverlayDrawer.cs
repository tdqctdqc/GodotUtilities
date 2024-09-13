
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotUtilities.Graphics;

public class MapOverlayDrawer
{
    private Func<Node> _getParent;
    private List<Node2D> _nodes;
    private int _z;

    public MapOverlayDrawer(int z, Func<Node> getParent)
    {
        _nodes = new List<Node2D>();
        _z = z;
        _getParent = getParent;
    }

    public void Clear()
    {
        foreach (var node in _nodes)
        {
            node.QueueFree();
        }
        _nodes.Clear();
    }

    public void Draw(Action<MeshBuilder> draw, Vector2 pos)
    {
        var mb = new MeshBuilder();
        draw(mb);
        var mi = mb.GetMeshInstance();
        AddNode(mi, pos);
    }

    public void Label(string text, Color color, Vector2 pos,
        float scale = 1f)
    {
        var label = NodeExt.CreateLabel(text);
        label.Modulate = color;
        label.Scale = Vector2.One * scale;
        var node = new Node2D();
        node.AddChild(label);
        AddNode(node, pos);
    }

    public void AddNode(Node2D node, Vector2 pos)
    {
        node.ZIndex = _z;
        node.Position = pos;
        _nodes.Add(node);
        _getParent().AddChild(node);
    }

    public void SetVisibility(bool vis)
    {
        foreach (var node in _nodes)
        {
            node.Visible = vis;
        }
    }
}