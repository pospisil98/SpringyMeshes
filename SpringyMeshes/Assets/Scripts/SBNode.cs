using System;
using System.Collections.Generic;
using UnityEngine;


public class SBNode
{
    private const float eps = 0.01f;
    private float mass;
    private State state;
    private Plane plane;

    private float c_r = 1.0f;
    private float c_f = 0.0f;

    private bool isFixed = false;
    
    private List<SBSDampedSpring> springs = new List<SBSDampedSpring>();

    public SBNode(Vector3 position, float mass)
    {
        state = new State(Vector3.zero, position, Vector3.zero);
        this.mass = mass;
        plane = new Plane(Vector3.zero, Vector3.up);
    }

    public void Tick(float deltaTime, Transform transform, Box box) {

        if (isFixed)
        {
            state.velocity = Vector3.zero;
            state.force = Vector3.zero;
            return;
        }

        // Determine accelerations by Newton’s second law
        Vector3 acceleration = state.force / mass;
        state.force = Vector3.zero;
        
        // new-state = Integration of accelerations over timestep delta
        State newState = new State(state.velocity, state.position);
        newState.Integrate(acceleration, deltaTime);

        
        // collision detection with triangles
        // for (int i = 0; i < box.triangles.Count; i++)
        // {
        //     if (box.triangles[i].intersectLineSegment(transform.TransformPoint(state.position), transform.TransformPoint(newState.position)))
        //     {
        //         float distance = box.triangles[i].pointDist(transform.TransformPoint(state.position));
        //         float newDistance = box.triangles[i].pointDist(transform.TransformPoint(newState.position));
        //
        //         if (newDistance < 0.0f)
        //         {
        //             // calculate first collision and reintegrate
        //             float f = distance / (distance - newDistance);
        //             float deltaTimeCollision = deltaTime * f;
        //             newState = new State(state.velocity, state.position);
        //             newState.Integrate(acceleration, deltaTimeCollision);
        //
        //             // collision response
        //             Vector3 v_n_minus = Vector3.Dot(newState.velocity, box.triangles[i].normal) * box.triangles[i].normal;
        //             Vector3 v_t_minus = newState.velocity - v_n_minus;
        //
        //             Vector3 v_n_plus = -c_r * Vector3.Dot(newState.velocity, box.triangles[i].normal) * box.triangles[i].normal;
        //             Vector3 v_t_plus = (1.0f - c_f) * v_t_minus;
        //
        //             newState.velocity = v_n_plus + v_t_plus;
        //             newState.velocity *= 0.01f;
        //             newState.Integrate(Vector3.zero, deltaTime - deltaTimeCollision);
        //         }
        //         break;
        //     }
        // }


        // // collision detection with plane
        float distance = plane.pointDist(transform.TransformPoint(state.position));
        float newDistance = plane.pointDist(transform.TransformPoint(newState.position));
        
        if (newDistance < 0.0f)
        {
            // calculate first collision and reintegrate
            float f = distance / (distance - newDistance);
            float deltaTimeCollision = deltaTime * f;
            newState = new State(state.velocity, state.position);
            newState.Integrate(acceleration, deltaTimeCollision);
        
            // collision response
            Vector3 v_n_minus = Vector3.Dot(newState.velocity, plane.normal) * plane.normal;
            Vector3 v_t_minus = newState.velocity - v_n_minus;
        
            Vector3 v_n_plus = -c_r * Vector3.Dot(newState.velocity, plane.normal) * plane.normal;
            Vector3 v_t_plus = (1.0f - c_f) * v_t_minus;
        
            newState.velocity = v_n_plus + v_t_plus;
            newState.Integrate(Vector3.zero, deltaTime - deltaTimeCollision);
        }
        
        // current-state = new-state
        state = newState;
        

    }

    public void AddForce(Vector3 force)
    {
        state.force += force;
    }

    public float Mass
    {
        get => mass;
        set => mass = value;
    }

    public Vector3 Position
    {
        get => state.position;
        set => state.position = value;
    }

    public Vector3 Force
    {
        get => state.force;
        set => state.force = value;
    }

    public Vector3 Velocity
    {
        get => state.velocity;
        set => state.velocity = value;
    }

    public bool IsFixed
    {
        get => isFixed;
        set => isFixed = value;
    }

    public List<SBSDampedSpring> Springs
    {
        get => springs;
        set => springs = value;
    }
}
