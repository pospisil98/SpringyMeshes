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
    

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.normal = Vector3.Cross(b - a, c - a).normalized;
    }
    
    bool pointInTriangle(Vector3 p)
    {
        float eps = 0.1f;
        // Translate point and triangle so that point lies at origin
        a -= p; b -= p; c -= p;
        float ab = Vector3.Dot(a, b);
        float ac = Vector3.Dot(a, c);
        float bc = Vector3.Dot(b, c);
        float cc = Vector3.Dot(c, c);
        // if (bc * ac - cc * ab < 0.0f) return false;
        Debug.Log("Point in tri A: " + (bc * ac - cc * ab));
        if (bc * ac - cc * ab + eps < 0.0f) return false;
        // Make sure plane normals for pab and pca point in the same direction
        float bb = Vector3.Dot(b, b);
        // if (ab * bc - ac * bb < 0.0f) return false;
        Debug.Log("Point in tri B: " + (ab * bc - ac * bb));
        if (ab * bc - ac * bb + eps < 0.0f) return false;
        // Otherwise P must be in (or on) the triangle
        return true;
    }

    public bool intersectPoint(Vector3 p, Vector3 v, float deltaTime, out float f)
    {

        float eps = 0.001f;
        f = 0;
        float tHit = Vector3.Dot(a - p, normal) / Vector3.Dot(v, normal);
        Debug.Log(tHit + " " + deltaTime);
        // Debug.Log((0.0f <= tHit).ToString() + " " + (tHit < deltaTime).ToString());
        
        if (0.0f <= tHit + eps && tHit < deltaTime + eps)
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
