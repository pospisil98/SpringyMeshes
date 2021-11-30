using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;
    public Vector3 normal;

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.normal = Vector3.Cross(b - a, c - a).normalized;
    }

    public bool intersectLineSegment(Vector3 p, Vector3 q)
    {
        Vector3 pq = q - p;
        Vector3 pa = a - p;
        Vector3 pb = b - p;
        Vector3 pc = c - p;
        
        Vector3 m = Vector3.Cross(pq, pc);
        float s = Vector3.Dot(m, c - b);
        float t = Vector3.Dot(m, a - c);
        float u = Vector3.Dot(pq, Vector3.Cross(c, b)) + s;
        if (u < 0.0f) return false;
        float v = Vector3.Dot(pq, Vector3.Cross(a, c)) + t;
        if (v < 0.0f) return false;
        float w = Vector3.Dot(pq, Vector3.Cross(b, a)) - s - t;
        if (w < 0.0f) return false;
        return true;
    }
    
    public float pointDist(Vector3 pointPosition)
    {
        return Vector3.Dot(pointPosition - a, normal);
    }
}
