using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Rendering.HybridV2;
using UnityEngine;

public class Triangle
{
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;
    public Vector3 normal;
    public int id;
    

    public Triangle(Vector3 a, Vector3 b, Vector3 c, int id)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.normal = Vector3.Cross(b - a, c - a).normalized;
        this.id = id;
    }
    
    bool pointInTriangle(Vector3 p)
    {
        float eps = 0.01f;
        
        // Compute vectors        
        Vector3 v0 = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = p - a;

        // Compute dot products
        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        // Compute barycentric coordinates
        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        // Check if point is in triangle
        return (u >= 0) && (v >= 0) && (u + v < 1);

    }

    public bool intersectPoint(Vector3 p, Vector3 v, float deltaTime, out float f)
    {

        float eps = 0.001f;
        f = 0;
        float tHit = Vector3.Dot(a - p, normal) / Vector3.Dot(v, normal);
        Debug.Log(id + ": " + tHit + " " + deltaTime);
        // Debug.Log((0.0f <= tHit).ToString() + " " + (tHit < deltaTime).ToString());
        
        // if (0.0f <= tHit + eps && tHit < deltaTime + eps)
        if (0.0f + eps <= tHit && tHit < deltaTime + eps)
        {
            Debug.Log("Collision with plane detected");
            Vector3 pHit = p + tHit * v;
            if (pointInTriangle(pHit))
            {
                Debug.Log("Collision with triangle detected");
                f = tHit;
                return true;
            }
        }
        return false;
    }
    
    public float pointDist(Vector3 pointPosition)
    {
        return Vector3.Dot(pointPosition - a, normal);
    }
    
    public float sphereDist(Vector3 spherePosition, float sphereRadius)
    {
        return Vector3.Dot(spherePosition - a, normal) - sphereRadius;
    }
}
