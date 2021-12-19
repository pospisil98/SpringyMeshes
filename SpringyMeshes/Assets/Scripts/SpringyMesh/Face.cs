using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Face
{
    public float angle1;
    public float angle2;
    public float angle3;

    public Vector3 normal;

    public Vertex v1;
    public Vertex v2;
    public Vertex v3;

    public Face(Vertex v1, Vertex v2, Vertex v3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
        Update();
    }

    public void Update()
    {
        angle1 = Vector3.Angle(v3.Position - v1.Position, v2.Position - v1.Position);
        angle2 = Vector3.Angle(v1.Position - v2.Position, v3.Position - v2.Position);
        angle3 = Vector3.Angle(v2.Position - v3.Position, v1.Position - v3.Position);
        
        normal =  Vector3.Cross(v3.Position - v1.Position, v2.Position - v1.Position).normalized;
    }
}