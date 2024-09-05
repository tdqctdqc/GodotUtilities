using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public static class TriangleExt 
{
    public static float GetDistFromEdge(Vector2 a, Vector2 b, Vector2 c, 
        Vector2 point)
    {
        if (ContainsPoint(a, b, c, point)) return 0f;
        var close1 = point.GetClosestPointOnLineSegment(a, b);
        var dist1 = point.DistanceTo(close1);
        var close2 = point.GetClosestPointOnLineSegment(a, c);
        var dist2 = point.DistanceTo(close2);
        var close3 = point.GetClosestPointOnLineSegment(c, b);
        var dist3 = point.DistanceTo(close3);
        var res = Mathf.Min(dist1, dist2);
        res = Mathf.Min(res, dist3);
        return res;
    }
    
    public static float GetMinAltitude(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        return Mathf.Min(p0.DistToLine(p1, p2), Mathf.Min(p1.DistToLine(p0, p2), p2.DistToLine(p0, p1)));
    }

    
    public static float GetApproxArea(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        var l0 = p0.DistanceTo(p1);
        var l1 = p1.DistanceTo(p2);
        var l2 = p2.DistanceTo(p0);
        var semiPerim = (l0 + l1 + l2) / 2f;
        var perimScore = semiPerim * (semiPerim - l0) * (semiPerim - l1) * (semiPerim - l2);
        if (perimScore < 0f)
        {
            return 0f;
        }
        var area = Mathf.Sqrt(semiPerim * (semiPerim - l0) * (semiPerim - l1) * (semiPerim - l2) );
        if (float.IsNaN(area)) throw new Exception($"bad tri area {p0} {p1} {p2} semi perims {l0} {l1} {l2}");
        return area;
    }
    
    
    public static bool ContainsPoint(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
    {

        return Geometry2D.IsPointInPolygon(p, new[] { a, b, c });
        // Calculate the barycentric coordinates
        // of point P with respect to triangle ABC
        double denominator = ((b[1] - c[1]) * (a[0] - c[0]) + (c[0] - b[0]) * (a[1] - c[1]));
        double aBary = ((b[1] - c[1]) * (p[0] - c[0]) + (c[0] - b[0]) * (p[1] - c[1])) / denominator;
        double bBary = ((c[1] - a[1]) * (p[0] - c[0]) + (a[0] - c[0]) * (p[1] - c[1])) / denominator;
        double cBary = 1 - aBary - bBary;
 
        // Check if all barycentric coordinates
        // are non-negative
        if (aBary >= 0 && bBary >= 0 && cBary >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}