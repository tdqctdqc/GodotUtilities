using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotUtilities.CSharpExt;

public static class Vector2Ext
{
    public static int Compare(this Vector2 a, Vector2 b)
    {
        if (a.X < b.X) return -1;
        if (a.X > b.X) return 1;
        if (a.Y < b.Y) return -1;
        if (a.Y > b.Y) return 1;
        return 0;
    }
    public static int Compare(this Vector2I a, Vector2I b)
    {
        if (a.X < b.X) return -1;
        if (a.X > b.X) return 1;
        if (a.Y < b.Y) return -1;
        if (a.Y > b.Y) return 1;
        return 0;
    }
    public static bool RangeOverlap(this Vector2 v, Vector2 w)
    {
        if (w.X <= v.X && v.X <= w.Y) return true;
        if (w.X <= v.Y && v.Y <= w.Y) return true;
        if (v.X <= w.X && w.X <= v.Y) return true;
        if (v.X <= w.Y && w.Y <= v.Y) return true;
        return false;
    }
    
    
    public static bool HasNaN(this Vector2 v)
    {
        return float.IsNaN(v.X) || float.IsNaN(v.Y);
    }
    public static bool HasNaN(this Vector2I v)
    {
        return float.IsNaN(v.X) || float.IsNaN(v.Y);
    }
    public static Vector2 RoundTo2Digits(this Vector2 v)
    {
        return new Vector2(v.X.RoundTo2Digits(), v.Y.RoundTo2Digits());
    }
    
    
    
    public static Vector2 Intify(this Vector2 v)
    {
        return new Vector2(Mathf.FloorToInt(v.X), Mathf.FloorToInt(v.Y));
    }
    public static Vector2 Avg(this List<Vector2> points)
    {
        var res = Vector2.Zero;
        points.ForEach(p => res += p);
        return res / points.Count;
    }
    public static Vector2 Avg(this IEnumerable<Vector2> v)
    {
        return Sum(v) / v.Count();
    }
    public static Vector2 Avg(this IEnumerable<Vector2I> v)
    {
        return Sum(v) / v.Count();
    }
    public static Vector2I Sum(this IEnumerable<Vector2I> v)
    {
        var r = Vector2I.Zero;
        foreach (var vector2 in v)
        {
            r += vector2;
        }

        return r;
    }
    public static Vector2 Sum(this IEnumerable<Vector2> v)
    {
        var r = Vector2.Zero;
        foreach (var vector2 in v)
        {
            r += vector2;
        }

        return r;
    }

    public static bool PointIsOnLine(this Vector2 point, Vector2 from, Vector2 to)
    {
        return from.Cross(to) == 0;
    }
    public static bool PointIsInLineSegment(this Vector2 point, Vector2 from, Vector2 to)
    {
        if (point.PointIsOnLine(from, to) == false) return false;
        return point.X >= Mathf.Min(from.X, to.X)
                && point.X <= Mathf.Max(from.X, to.X)
                && point.Y >= Mathf.Min(from.Y, to.Y)
                && point.Y <= Mathf.Max(from.Y, to.Y);
    }
    private static bool OnSegment(Vector2 p, Vector2 q, Vector2 r) 
    { 
        if (q.X <= Mathf.Max(p.X, r.X) && q.X >= Mathf.Min(p.X, r.X) && 
            q.Y <= Mathf.Max(p.Y, r.Y) && q.Y >= Mathf.Min(p.Y, r.Y)) 
            return true; 
  
        return false; 
    } 
  
    private static int Orientation(Vector2 p, Vector2 q, Vector2 r) 
    { 
        // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
        // for details of below formula. 
        float val = (q.Y - p.Y) * (r.X - q.X) - 
                  (q.X - p.X) * (r.Y - q.Y); 
  
        if (val == 0) return 0;  // collinear 
  
        return (val > 0)? 1: 2; // clock or counterclock wise 
    } 
    public static bool LineSegIntersect(Vector2 p0, Vector2 p1, 
        Vector2 q0, Vector2 q1, bool inclusive, out Vector2 intersectPoint)
    {
        intersectPoint = Vector2.Zero;
        
        if (inclusive == false)
        {
            if (p0 == q0 || p0 == q1 || p1 == q0 || p1 == q1)
            {
                return false;
            }
        }

        if (intersect(p0, p1, q0, q1))
        {
            var res = Geometry2D.LineIntersectsLine(p0, p1 - p0, q0, q1 - q0);
            if (res.Obj == null) return false;
            intersectPoint = (Vector2)Geometry2D.LineIntersectsLine(p0, p1 - p0, q0, q1 - q0).Obj;
            return true;
        }
        return false;
    }
    private static bool intersect(Vector2 p1, Vector2 q1, 
        Vector2 p2, Vector2 q2) 
    { 
        // Find the four orientations needed for general and 
        // special cases 
        int o1 = Orientation(p1, q1, p2); 
        int o2 = Orientation(p1, q1, q2); 
        int o3 = Orientation(p2, q2, p1); 
        int o4 = Orientation(p2, q2, q1); 
  
        // General case 
        if (o1 != o2 && o3 != o4) 
            return true;
  
        // Special Cases 
        // p1, q1 and p2 are collinear and p2 lies on segment p1q1 
        if (o1 == 0 && OnSegment(p1, p2, q1)) return true; 
  
        // p1, q1 and q2 are collinear and q2 lies on segment p1q1 
        if (o2 == 0 && OnSegment(p1, q2, q1)) return true; 
  
        // p2, q2 and p1 are collinear and p1 lies on segment p2q2 
        if (o3 == 0 && OnSegment(p2, p1, q2)) return true; 
  
        // p2, q2 and q1 are collinear and q1 lies on segment p2q2 
        if (o4 == 0 && OnSegment(p2, q1, q2)) return true; 
  
        return false; // Doesn't fall in any of the above cases 
    }
    public static Vector2 ClampToBox(this Vector2 p, Vector2 bound1, Vector2 bound2)
    {
        var minXBound = Mathf.Min(bound1.X, bound2.X);
        var minYBound = Mathf.Min(bound1.Y, bound2.Y);
        var maxXBound = Mathf.Max(bound1.X, bound2.X);
        var maxYBound = Mathf.Max(bound1.Y, bound2.Y);

        return new Vector2(Mathf.Clamp(p.X, minXBound, maxXBound),
            Mathf.Clamp(p.Y, minYBound, maxYBound));
    }
    public static float DistToLine(this Vector2 point, Vector2 start, Vector2 end)
    {
        var theta = Mathf.Abs((point - start).AngleTo(end - start));
        return Mathf.Sin(theta) * point.DistanceTo(start);
        
        // vector AB
        var AB = new Vector2();
        AB.X = end.X - start.X;
        AB.Y = end.Y - start.Y;
 
        // vector BP
        var BE = new Vector2();
        BE.X = point.X - end.X;
        BE.Y = point.Y - end.Y;
 
        // vector AP
        var AE = new Vector2();
        AE.X = point.X - start.X;
        AE.Y = point.Y - start.Y;
 
        // Variables to store dot product
        float AB_BE, AB_AE;
 
        // Calculating the dot product
        AB_BE = (AB.X * BE.X + AB.Y * BE.Y);
        AB_AE = (AB.X * AE.X + AB.Y * AE.Y);
 
        // Minimum distance from
        // point E to the line segment
        float reqAns = 0;
 
        // Case 1
        if (AB_BE > 0)
        {
 
            // Finding the magnitude
            var y = point.Y - end.Y;
            var x = point.X - end.X;
            reqAns = Mathf.Sqrt(x * x + y * y);
        }
 
        // Case 2
        else if (AB_AE < 0)
        {
            var y = point.Y - start.Y;
            var x = point.X - start.X;
            reqAns = Mathf.Sqrt(x * x + y * y);
        }
 
        // Case 3
        else
        {
 
            // Finding the perpendicular distance
            var x1 = AB.X;
            var y1 = AB.Y;
            var x2 = AE.X;
            var y2 = AE.Y;
            var mod = Mathf.Sqrt(x1 * x1 + y1 * y1);
            reqAns = Mathf.Abs(x1 * y2 - y1 * x2) / mod;
        }
        return reqAns;
    }
    public static Vector2 GetPointAlongLine(this IList<Vector2> points,
        Func<Vector2, Vector2, Vector2> getOffset,
        float ratio)
    {
        if (ratio < 0f || ratio > 1f)
        {
            throw new Exception("ratio is " + ratio);
        }
        var totalLength = 0f;
        for (var i = 0; i < points.Count - 1; i++)
        {
            totalLength += getOffset(points[i], points[i + 1]).Length();
        }

        var targetLength = ratio * totalLength;
        var soFar = 0f;
        for (var i = 0; i < points.Count - 1; i++)
        {
            var from = points[i];
            var to = points[i + 1];
            var offset = getOffset(from, to);
            var toGo = targetLength - soFar;
            if (toGo > offset.Length())
            {
                soFar += offset.Length();
                continue;
            }

            return from + offset.Normalized() * toGo;
        }

        return points.Last();
    }
    
    
    public static List<Vector2> GetSubline(this IList<Vector2> points,
        Func<Vector2, Vector2, Vector2> getOffset,
        float startRatio, float endRatio)
    {
        if (startRatio < 0f || startRatio > 1f || endRatio < 0f || endRatio > 1f)
        {
            throw new Exception($"ratio out of bounds, from {startRatio} to {endRatio}");
        }
        
        var totalLength = 0f;
        for (var i = 0; i < points.Count - 1; i++)
        {
            totalLength += getOffset(points[i], points[i + 1]).Length();
        }

        var res = new List<Vector2>();
        var ratioSoFar = 0f;
        for (var i = 0; i < points.Count - 1; i++)
        {
            if (ratioSoFar > endRatio) break;
            var from = points[i];
            var to = points[i + 1];
            var offset = getOffset(from, to);
            var nextRatio = ratioSoFar + offset.Length() / totalLength;

            if (ratioSoFar >= startRatio && ratioSoFar <= endRatio)
            {
                addToRes(from);
            }
            if (ratioSoFar < startRatio && startRatio < nextRatio)
            {
                var alongSegRatio = (startRatio - ratioSoFar) / (nextRatio - ratioSoFar);
                var startP = from + (to - from) * alongSegRatio;
                addToRes(startP);
            }
            if (ratioSoFar < endRatio && endRatio < nextRatio)
            {
                var alongSegRatio = (endRatio - ratioSoFar) / (nextRatio - ratioSoFar);
                var endP = from + (to - from) * alongSegRatio;
                addToRes(endP);
            }
            if (nextRatio >= startRatio && nextRatio <= endRatio)
            {
                addToRes(to);
            }

            ratioSoFar = nextRatio;
            
        }

        void addToRes(Vector2 p)
        {
            if (res.Count > 0 && res[res.Count - 1] == p) return;
            res.Add(p);
        }

        return res;
    }
    
    public static Vector2 GetPointAlongCircle(this IList<Vector2> points,
        Func<Vector2, Vector2, Vector2> getOffset,
        float ratio)
    {
        if (ratio < 0f || ratio > 1f) throw new Exception();
        var totalLength = 0f;
        for (var i = 0; i < points.Count; i++)
        {
            totalLength += getOffset(points[i], points.Modulo(i + 1)).Length();
        }

        var targetLength = ratio * totalLength;
        var soFar = 0f;
        for (var i = 0; i <= points.Count; i++)
        {
            var from = points.Modulo(i);
            var to = points.Modulo(i + 1);
            var offset = getOffset(from, to);
            var toGo = targetLength - soFar;
            if (toGo > offset.Length())
            {
                soFar += offset.Length();
                continue;
            }

            return from + offset.Normalized() * toGo;
        }

        throw new Exception();
    }

    public static Vector2 GetClosestPointOnLine(this Vector2 point, Vector2 origin, Vector2 direction)
    {
        direction = direction.Normalized();
        Vector2 lhs = point - origin;
        float dotP = lhs.Dot(direction);
        return origin + direction * dotP;
    }
    public static Vector2 GetClosestPointOnLineSegment
        (this Vector2 point, 
            Vector2 from, Vector2 to)
    {
        var origin = from;
        var direction = to - from;
        Vector2 lhs = point - origin;
        float dotP = lhs.Dot(direction);
        var closeOnLine = point.GetClosestPointOnLine(origin, direction);
        
        var test = closeOnLine - from;
        if (test.LengthSquared() > direction.LengthSquared()
                || Mathf.Abs(direction.AngleTo(test)) > 0.1f)
        {
            return closeOnLine.DistanceTo(from) <= closeOnLine.DistanceTo(to)
                ? from
                : to;
        }
        return closeOnLine;
    }

    public static (Vector2, Vector2) Order(this Vector2 v, Vector2 w)
    {
        if (v.X < w.X)
        {
            return (v, w);
        }
        if (w.X < v.X)
        {
            return (w, v);
        }
        if (v.Y < w.Y)
        {
            return (v, w);
        }
        return (w, v);
    }

    public static Vector2[]? ClipPolygonByLine(this Vector2[] polygon, 
        Vector2 clipPoint, Vector2 clipLineDir, Vector2 keepDirection,
        out Vector2[] keepBox,
        float keepDist = 1000f)
    {
        keepBox = new Vector2[]
        {
            clipPoint + clipLineDir.Normalized() * keepDist,
            clipPoint - clipLineDir.Normalized() * keepDist,
            clipPoint - clipLineDir.Normalized() * keepDist + keepDirection.Normalized() * keepDist,
            clipPoint + clipLineDir.Normalized() * keepDist + keepDirection.Normalized() * keepDist,
        };
        var newPolys = Geometry2D.IntersectPolygons(polygon, keepBox);
        if (newPolys.Count != 1) throw new Exception();
        return newPolys[0];
    }
    public static float GetArea(this Vector2[] boundaryPoints)
    {
        if (boundaryPoints.Length < 3) return 0f;
        var tris = Geometry2D.TriangulatePolygon(boundaryPoints);
        var area = 0f;
        for (var i = 0; i < tris.Length; i+=3)
        {
            var a = boundaryPoints[tris[i]];
            var b = boundaryPoints[tris[i+1]];
            var c = boundaryPoints[tris[i+2]];
            area += TriangleExt.GetApproxArea(a, b, c);
        }

        return area;
    }
}
