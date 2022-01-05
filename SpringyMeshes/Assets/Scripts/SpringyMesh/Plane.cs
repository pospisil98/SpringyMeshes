using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane
{
    public Vector3 position;
    public Vector3 normal;

    public Plane(Vector3 position, Vector3 normal)
    {
        this.position = position;
        this.normal = normal;
    }

    public float pointDist(Vector3 pointPosition)
    {
        return Vector3.Dot(pointPosition - position, normal);
    }

    public float sphereDist(Vector3 spherePosition, float sphereRadius)
    {
        return Vector3.Dot(spherePosition - position, normal) - sphereRadius;
    }
}