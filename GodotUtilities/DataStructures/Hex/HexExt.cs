using System.Runtime.InteropServices;
using Godot;

namespace GodotUtilities.DataStructures.Hex;

public static class HexExt
{
    public const float HexHeight = 0.866025f;
    public static Vector3I North {get; private set;} = new Vector3I(0,1,-1);
    public static Vector3I NorthEast {get; private set;} = new Vector3I(1,0,-1); 
    public static Vector3I SouthEast {get; private set;} = new Vector3I(1,-1,0); 
    public static Vector3I South {get; private set;} = new Vector3I(0,-1,1); 
    public static Vector3I SouthWest {get; private set;} = new Vector3I(-1,0,1); 
    public static Vector3I NorthWest {get; private set;} = new Vector3I(-1,1,0);
    
    public static List<Vector3I> HexDirs { get; private set; }
        = [ North, NorthEast, SouthEast, South, SouthWest, NorthWest ];
    
    public static List<Vector2> HexRadii { get; private set; }
        = [ Vector2.Up.Rotated(Mathf.Pi / 6f), Vector2.Up.Rotated(3f * Mathf.Pi / 6f),
            Vector2.Up.Rotated(5f * Mathf.Pi / 6f), Vector2.Up.Rotated(7f * Mathf.Pi / 6f),
            Vector2.Up.Rotated(9f * Mathf.Pi / 6f), Vector2.Up.Rotated(11f * Mathf.Pi / 6f), ];

    

    public static IEnumerable<Vector3I> GetNeighbors(
        this Vector3I hex)
    {
        yield return hex + North;
        yield return hex + NorthEast;
        yield return hex + SouthEast;
        yield return hex + South;
        yield return hex + SouthWest;
        yield return hex + NorthWest;
    }
    public static Vector2I CubeToGridCoords(this Vector3I cube)
    {
        int gridCoordsX = cube.X;
        int gridCoordsY = cube.Y + (int)(  ( cube.X + ((uint)cube.X & 1) )/2  );
        Vector2I gridCoords = new Vector2I(gridCoordsX, gridCoordsY);
        return gridCoords; 
    }
    public static Vector3I GridCoordsToCube(this Vector2I gridCoords) 
    {
        int cubeX = gridCoords.X; 
        int cubeY = gridCoords.Y - (int)((gridCoords.X + ((uint)gridCoords.X & 1) )/2  );
        int cubeZ = -cubeX - cubeY; 
        Vector3I cube = new Vector3I(cubeX, cubeY, cubeZ);
        return cube; 
    }
    public static IEnumerable<Vector3I> 
        GetHexesInRadius(this Vector3I hex, int radius)
    {

        for (int i = -radius; i < radius + 1; i++)
        {
            for (   int j = Mathf.Max(-radius, -i - radius); 
                        j < Mathf.Min(radius, -i + radius) + 1; 
                        j++)
            {
                int x = i + hex.X;
                int y = j + hex.Y;
                int z = -x-y;
                yield return new Vector3I(x,y,z);
            }
        }
    }

    public static Vector2 GetWorldPos(this Vector3I coords)
    {
        return coords.CubeToGridCoords().GetWorldPos();
    }
    
    public static Vector2 GetWorldPos(
        this Vector2I gridCoords)
    {   
        var pos = new Vector2(gridCoords.X * 1.5f, gridCoords.Y * .866f * 2f);
        if(gridCoords.X % 2 != 1)
        {
            pos.Y += .866f;
        }
        return pos;
    }
    public static float GetHexAngle(this Vector3I h1, Vector3I h2)
    {
        var p1 = GetWorldPos(
            CubeToGridCoords(h1));
        var p2 = GetWorldPos(
            CubeToGridCoords(h2));
        return .5f * Mathf.Pi - (p2 - p1).AngleTo(Vector2.Up);
    }
    public static int GetHexDistance(this Vector3I h1, Vector3I h2)
    {
        return (Math.Abs(h1.X - h2.X) 
                      + Math.Abs(h1.Y - h2.Y) 
                      + Math.Abs(h1.Z - h2.Z)) / 2;
    }

    public static long GetKeyFromGridCoord(this Vector2I coord)
    {
        var xBytes = BitConverter.GetBytes(coord.X);
        var yBytes = BitConverter.GetBytes(coord.Y);
        var keyBytes = new byte[8];
        System.Buffer.BlockCopy(xBytes, 0, keyBytes, 0, 4);
        System.Buffer.BlockCopy(yBytes, 0, keyBytes, 4, 4);
        var key = BitConverter.ToInt64(keyBytes);
        if (key == long.MinValue) throw new Exception();
        return key;
    }
    public static Vector2I GetGridCoordFromKey(this long key)
    {
        var b1 = BitConverter.GetBytes(key);
        var b2 = new byte[4];
        var b3 = new byte[4];
        System.Buffer.BlockCopy(b1, 0, b2, 0, 4);
        System.Buffer.BlockCopy(b1, 4, b3, 0, 4);
        return new Vector2I(BitConverter.ToInt32(b2), BitConverter.ToInt32(b3)); 
    }

    public static Vector3I GetCubeCoordFromKey(this long key)
    {
        return key.GetGridCoordFromKey().GridCoordsToCube();
    }

    public static bool IsPointInHex(Vector2 p, Vector2 hexPos, float radius)
    {
        return TriangleExt.ContainsPoint(Vector2.Zero,
                   hexPos + HexRadii[0] * radius, hexPos + HexRadii[1] * radius, p)
               || TriangleExt.ContainsPoint(Vector2.Zero,
                   hexPos + HexRadii[1] * radius, hexPos + HexRadii[2] * radius, p)
               || TriangleExt.ContainsPoint(Vector2.Zero,
                   hexPos + HexRadii[2] * radius, hexPos + HexRadii[3] * radius, p)
               || TriangleExt.ContainsPoint(Vector2.Zero,
                   hexPos + HexRadii[3] * radius, hexPos + HexRadii[4] * radius, p)
               || TriangleExt.ContainsPoint(Vector2.Zero,
                   hexPos + HexRadii[4] * radius, hexPos + HexRadii[5] * radius, p)
               || TriangleExt.ContainsPoint(Vector2.Zero,
                   hexPos + HexRadii[5] * radius, hexPos + HexRadii[0] * radius, p);
    }
    
}