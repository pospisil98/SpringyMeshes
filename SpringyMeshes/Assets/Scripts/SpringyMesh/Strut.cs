using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strut
{
    public float k;
    public float d;
    public float l0;
    public float kTheta;

    public Vertex from;
    public Vertex to;

    public Face f1;
    public Face f2;

    public Vertex opposite1;
    public Vertex opposite2;


    public Strut(float k, float d, Vertex from, Vertex to)
    {
        this.k = k;
        this.d = d;

        this.from = from;
        this.to = to;

        this.l0 = Vector3.Distance(from.Position, to.Position);
        //TODO: fix
        this.kTheta = 0;
    }

    public bool IsSame(int id1, int id2)
    {
        return (from.id == id1 && to.id == id2) || (@from.id == id2 && to.id == id1);
    }

    public void ApplyForces()
    {
        float length = Vector3.Distance(from.Position, to.Position);

        Vector3 dir = (to.Position - from.Position).normalized;

        // apply spring forces
        Vector3 f_s = k * (length - l0) * dir;
        from.AddForce(f_s);
        to.AddForce(-f_s);

        // apply damping force
        Vector3 f_d = d * Vector3.Dot((to.Velocity - from.Velocity), dir) * dir;
        from.AddForce(f_d);
        to.AddForce(-f_d);
        
        // TODO: computation of hinge forces
    }
}


