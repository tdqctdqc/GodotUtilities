using Godot;

namespace GodotUtilities.DataStructures.Hex;

public static class HexExt
{
    public static Vector3I North {get; private set;} = new Vector3I(0,1,-1);
    public static Vector3I NorthEast {get; private set;} = new Vector3I(1,0,-1); 
    public static Vector3I SouthEast {get; private set;} = new Vector3I(1,-1,0); 
    public static Vector3I South {get; private set;} = new Vector3I(0,-1,1); 
    public static Vector3I SouthWest {get; private set;} = new Vector3I(-1,0,1); 
    public static Vector3I NorthWest {get; private set;} = new Vector3I(-1,1,0);
    
    public static List<Vector3I> HexDirs { get; private set; }
        = new List<Vector3I>(){North, 
            NorthEast, SouthEast, 
            South, SouthWest, NorthWest};
    

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

    public static long GetKey(this Vector3I coord)
    {
        const long max = 2_097_152;
        const long maxSq = max * max;
        var x = (long)coord.X;
        var y = (long)coord.Y;
        var z = (long)coord.Z;

        checked
        {
            var xCheck = x * maxSq;
            var yCheck = y * maxSq;
            var zCheck = z * maxSq;

            return x + max * y + maxSq * z;
        }
    }
    
}