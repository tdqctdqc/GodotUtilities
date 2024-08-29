using Godot;

namespace GodotUtilities.DataStructures.Hex;

public readonly struct HexEdgeKey
{
    public Vector3I Lo { get; }
    public Vector3I Hi { get; }

    public HexEdgeKey(Vector3I a, Vector3I b)
    {
        (Lo, Hi) = Sort(a, b);
    }

    public static (Vector3I lo, Vector3I hi) Sort(Vector3I a, Vector3I b)
    {
        if (a.X < b.X) return (a, b);
        if (b.X < a.X) return (b, a);
        
        if (a.Y < b.Y) return (a, b);
        if (b.Y < a.Y) return (b, a);
        
        if (a.Z < b.Z) return (a, b);
        if (b.Z < a.Z) return (b, a);

        return (a, b);
    }
    
    
}