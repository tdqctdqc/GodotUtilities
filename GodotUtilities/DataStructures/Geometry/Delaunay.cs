using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DelaunatorSharp;
using Godot;
using GodotUtilities.DataStructures.Graph;

namespace GodotUtilities.DataStructures.Geometry;

public static class Delaunay
{
    public static int NextHalfEdge(this Delaunator d, int e)
    {
        return (e % 3 == 2) ? e - 2 : e + 1;
    }
    public static int PrevHalfEdge(this Delaunator d, int e)
    {
        return (e % 3 == 0) ? e + 2 : e - 1; 
    }

    public static int TriangleOfEdge(int e)
    {
        return Mathf.FloorToInt(e / 3);
    }
    public class DelaunatorPoint : IPoint
    {
        public double X {get; set;}
        public double Y {get; set;}

        public DelaunatorPoint(Vector2 v)
        {
            X = (int)v.X;
            Y = (int)v.Y;
        }
    }
    public static IPoint GetIPoint(this Vector2 v)
    {
        return new DelaunatorPoint(v);
    }
    public static Vector2 GetV2(this IPoint p)
    {
        return new Vector2((float) p.X, (float) p.Y);
    }
    public static Vector2 GetIntV2(this IPoint p)
    {
        return new Vector2(Mathf.FloorToInt((float)p.X), Mathf.FloorToInt((float)p.Y));
    }
    
    public static Graph<T, TEdge>
        GetVoronoiGraph<T, TEdge>(
            List<T> ts, 
            Func<T, Vector2> getPos,
            Func<T, T, TEdge> getEdge)
    {
        var dic = ts
            .ToDictionary(t => getPos(t).GetIPoint(), t => t);
        var ps = dic.Keys.ToArray();
        var delaunay = new Delaunator(ps);

        var graph = new Graph<T, TEdge>();
        foreach (var t in ts)
        {
            graph.AddNode(t);
        }
        foreach (var edge in delaunay.GetEdges())
        {
            var e1 = edge.Index;
            var p1 = delaunay.Points[delaunay.Triangles[e1]];
            var e2 = delaunay.Halfedges[e1];
            if (e2 == -1)
            {
                continue;
            }
            var p2 = delaunay.Points[delaunay.Triangles[e2]];
            var t1 = dic[p1];
            var t2 = dic[p2];
            graph.AddEdge(t1, t2, getEdge(t1, t2));
        }
        
        return graph;
    }
}