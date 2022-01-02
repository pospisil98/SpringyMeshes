using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public class State
{
    public Vector3 velocity;
    public Vector3 position;
    public Vector3 force;
    
    public State()
    {
        this.velocity = Vector3.zero;
        this.position = Vector3.zero;
        this.force = Vector3.zero;
    }

    public State(Vector3 velocity, Vector3 position)
    {
        this.velocity = velocity;
        this.position = position;
        this.force = Vector3.zero;
    }
    public State(Vector3 velocity, Vector3 position, Vector3 force)
    {
        this.velocity = velocity;
        this.force = force;
        this.position = position;
    }

    public void Integrate(Vector3 acceleration, float deltaTime)
    {
        // Euler
        // Vector3 prevVelocity = velocity;
        // velocity = velocity + acceleration * deltaTime;
        // position = position + (prevVelocity + velocity) * 0.5f * deltaTime;
        
        // Leap frog
        Vector3 prevVelocity = velocity;
        
        Vector3 newPosition = position + prevVelocity * deltaTime * 0.5f;
        Vector3 newVelocity = prevVelocity + acceleration * deltaTime;
        newPosition = newPosition + prevVelocity * deltaTime * 0.5f;

        velocity = newVelocity;
        position = newPosition;
    }
}
