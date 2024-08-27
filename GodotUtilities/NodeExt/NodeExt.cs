using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class NodeExt
{
    public static IEnumerable<Node> GetDescendents(this Node n)
    {
        foreach (var child in n.GetChildren())
        {
            yield return child;
            foreach (var descendent in child.GetDescendents())
            {
                yield return descendent;
            }
        }
    }
    
    public static List<Node> GetChildList(this Node n)
    {
        var l = new List<Node>();
        var children = n.GetChildren();
        foreach (var child in children)
        {
            l.Add((Node)child);
        }

        return l;
    }
    public static void AssignChildNode<T>(this Node n, ref T node, string name) where T : Node
    {
        node = (T) n.FindChild(name);
        if (node == null) throw new Exception();
    }
    public static void ChildAndCenterOn(this Node2D parent, Control toCenter, Vector2 parentDim)
    {
        parent.AddChild(toCenter);
        toCenter.Position = -parentDim / 2f;
    }

    public static Label CreateLabel(string text)
    {
        var l = new Label();
        l.Text = text;
        return l;
    }
    public static Label CreateLabelAsChild(this Node parent, string text)
    {
        var label = new Label();
        label.Text = text;
        parent.AddChild(label);
        return label;
    }
    
    public static bool Toggle(this Node2D n)
    {
        n.Visible = n.Visible == false;
        return n.Visible;
    }

    public static void ClearChildren(this Node n)
    {
        if (n == null) throw new Exception();
        while (n.GetChildCount() > 0)
        {
            var c = n.GetChild(0);
            n.RemoveChild(c);
            c.QueueFree();
        }
    }
    public static void UnparentChildren(this Node n)
    {
        if (n == null) throw new Exception();
        while (n.GetChildCount() > 0)
        {
            var c = n.GetChild(0);
            n.RemoveChild(c);
        }
    }
    public static void AddChildWithVSeparator(this Node parent, Node n)
    {
        parent.AddChild(n);
        var s = new VSeparator();
        parent.AddChild(s);
    }

    public static int GetTotalNumberOfNodesInSubTree(this Node head)
    {
        var res = 1;
        foreach (var c in head.GetChildren())
        {
            res += c.GetTotalNumberOfNodesInSubTree();
        }
        return res;
    }
}