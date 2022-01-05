using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Face
{
    public int id;
    public float angle1;
    public float angle2;
    public float angle3;

    public float initialArea;

    public Vector3 normal;

    public Vertex v1;
    public Vertex v2;
    public Vertex v3;

    public Face(int id, Vertex v1, Vertex v2, Vertex v3)
    {
        this.id = id;
        
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
        
        initialArea = AreaOfTriangle(v1.Position, v2.Position, v3.Position);
        
        Update();
    }

    public void Update()
    {
        angle1 = Vector3.Angle(v3.Position - v1.Position, v2.Position - v1.Position);
        angle2 = Vector3.Angle(v1.Position - v2.Position, v3.Position - v2.Position);
        angle3 = Vector3.Angle(v2.Position - v3.Position, v1.Position - v3.Position);
        
        normal =  Vector3.Cross( v2.Position - v1.Position,v3.Position - v1.Position).normalized;
    }

    public float GetAngleByVertex(Vertex v)
    {
        if (v == v1) { return angle1; }
        if (v == v2) { return angle2; }
        if (v == v3) { return angle3; }
        
        Debug.LogError("Given vertex is not present in triangle.");
        return -1;
    }

    private float AreaOfTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        return 0.5f * Vector3.Cross(b-a, c-a).magnitude;
    }
}