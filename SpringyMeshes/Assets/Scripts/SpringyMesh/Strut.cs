using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strut
{
    public int id;
    public float k;
    public float d;
    public float restLength;
    private float restAngle;
    public float kTheta;
    public float dTheta;

    public Vertex from;
    public Vertex to;

    public Face face1;
    public Face face2;

    public Vertex opposite1;
    public Vertex opposite2;


    public Strut(int id, Vertex from, Vertex to)
    {
        this.id = id;

        this.from = from;
        this.to = to;

        this.restLength = Vector3.Distance(from.Position, to.Position);
    }

    public void Preprocess(float avgLength)
    {        
        float P = 0.2f;
        float T = 0.1f;

        k = 4.0f * Mathf.PI * Mathf.PI * from.mass / (P * P);
        d = 2.0f * from.mass / T;

        // k = 50.0f;
        // d = 3.0f;
        k = 1150.0f;
        d = 3.0f;
        d *= restLength / avgLength;
        k *= restLength / avgLength;
        Vector3 h = (to.Position - from.Position).normalized;
        
        // this.restAngle = Vector3.Angle(face1.normal, face2.normal);
        this.restAngle = Mathf.Atan2(Vector3.Dot(Vector3.Cross(face1.normal, face2.normal), h), Vector3.Dot(face1.normal, face2.normal));
        
        // float Ttheta = 0.5f;
        // float Ptheta = 10.0f;
        float Ttheta = 0.5f;
        float Ptheta = 8.0f;
        
        Vector3 x02 = from.Position - opposite1.Position;
        Vector3 x03 = from.Position - opposite2.Position;
        
        Vector3 r_l = x02 - Vector3.Dot(x02, h) * h;
        Vector3 r_r = x03 - Vector3.Dot(x03, h) * h;
        float avgDist = 0.5f * (Vector3.Magnitude(r_l) + Vector3.Magnitude(r_r));

        float mass1 = opposite1.mass;
        float mass2 = opposite2.mass;
        float avgMass = 0.5f * (mass1 + mass2);
        
        dTheta = 2.0f * avgMass * avgDist / Ttheta;
        kTheta = 4.0f * Mathf.PI * Mathf.PI * avgDist * avgDist * avgMass / (Ptheta * Ptheta);        
        Debug.Log(k + " " + d);       
        // Debug.Log(kTheta + " " + dTheta);
        // tyhle jsou fajn
        // k = 25.0f;
        // d = 8.0f;
        // kTheta = 0.04f * 8.0f;
        // dTheta = 0.4f * 10.0f;        
        // k = 0.0f;
        // d = 0.0f;
        // kTheta = 2.0f;
        // dTheta = 0.1f;
        // k = 50.0f;
        // d = 3.0f;
        // kTheta = 20.02f;
        // dTheta = 1.0f;

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
        Vector3 f_s = k * (length - restLength) * dir;
        from.AddForce(f_s);
        to.AddForce(-f_s);

        // apply damping force
        Vector3 f_d = d * Vector3.Dot((to.Velocity - from.Velocity), dir) * dir;
        from.AddForce(f_d);
        to.AddForce(-f_d);
        
        // computation of hinge forces

        Vector3 h = (to.Position - from.Position).normalized;
        // Vector3 x02 = opposite1.Position - from.Position;
        // Vector3 x03 = opposite2.Position - from.Position;
        // Vector3 h = (from.Position - to.Position).normalized;
        Vector3 x02 = from.Position - opposite1.Position;
        Vector3 x03 = from.Position - opposite2.Position;
        
        // vectors formed by lofting a perpendicular from the hinge edge
        Vector3 r_l = x02 - Vector3.Dot(x02, h) * h;
        Vector3 r_r = x03 - Vector3.Dot(x03, h) * h;

        Vector3 n_l = face1.normal;
        Vector3 n_r = face2.normal;

        float theta = Mathf.Atan2(Vector3.Dot(Vector3.Cross(n_l, n_r), h), Vector3.Dot(n_l, n_r));
        // float theta = Vector3.Angle(n_l, n_r);

        Vector3 tau_k = kTheta * (theta - restAngle) * h;

        // float theta_l = Vector3.Dot(opposite1.Velocity, n_l) / Vector3.Magnitude(r_l);
        // float theta_r = Vector3.Dot(opposite2.Velocity, n_r) / Vector3.Magnitude(r_r);        
        // float theta_l = Vector3.Angle(Vector3.Dot(opposite1.Velocity, n_l) * n_l + r_l, r_l);
        // float theta_r = Vector3.Angle(Vector3.Dot(opposite2.Velocity, n_r) * n_r + r_r, r_r);

        float d02 = Vector3.Dot(x02, h);
        float d03 = Vector3.Dot(x03, h);
        
        // test
        Vector3 hinge_velocity_left;
        Vector3 hinge_velocity_right;
        float l01 = (to.Position - from.Position).magnitude;
        float fraction_vel_left = d02 / l01;
        float fraction_vel_right = d03 / l01;
        
        hinge_velocity_left = ((1.0f - fraction_vel_left) * from.Velocity) + (fraction_vel_left * to.Velocity);
        hinge_velocity_right = ((1.0f - fraction_vel_right) * from.Velocity) + (fraction_vel_right * to.Velocity);
        float theta_l = Vector3.Dot(opposite1.Velocity - hinge_velocity_left, n_l) / r_l.magnitude;
        float theta_r = Vector3.Dot(opposite2.Velocity - hinge_velocity_right, n_r) / r_r.magnitude;

        Vector3 tau_d = -dTheta * (theta_l + theta_r) * h;

        Vector3 tau = tau_k + tau_d;

        Vector3 f2 = n_l * Vector3.Dot(tau, h) / Vector3.Magnitude(r_l);
        Vector3 f3 = n_r * Vector3.Dot(tau, h) / Vector3.Magnitude(r_r);
        
        Vector3 f1 = -(d02 * f2 + d03 * f3) / Vector3.Magnitude(to.Position - from.Position);
        Vector3 f0 = -(f1 + f2 + f3);
        
        // Debug.Log(id + " theta l, r: " + theta_l + " " + theta_r);
        // Debug.Log(id + " tau: " + tau_k + " " + tau_d);
        // Debug.Log(id + " force: " + f0 + " " + f1 + " " + f2 + " " + f3);

        from.AddForce(f0);
        to.AddForce(f1);
        opposite1.AddForce(f2);
        opposite2.AddForce(f3);
    }
}


