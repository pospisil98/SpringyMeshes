using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBall : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject trajectoryObject;
    
    public float mass = 1.0f;
    public Vector3 force= Vector3.zero;
    public State state;
    public Vector3 windVelocity = Vector3.zero;
    public float airResistance = 0.4f; // air resistance constant
    public float c_r = 1.0f;
    public float c_f = 0.0f;

    public Vector3 initialVelocity = Vector3.zero;

    private float dilation = 1.0f;
    private Plane plane;
    
    void Start()
    {
        state = new State(initialVelocity, transform.position);
        plane = new Plane(Vector3.zero, Vector3.up);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime * dilation;
        
        // Determine accelerations by Newton’s second law
        Vector3 acceleration = Physics.gravity + (airResistance / mass) * (windVelocity - state.velocity);
        
        // new-state = Integration of accelerations over timestep delta
        State newState = new State(state.velocity, state.position);
        newState.Integrate(acceleration, deltaTime);

        // collision detection with plane
        float distance = plane.sphereDist(state.position, 0.25f);
        float newDistance = plane.sphereDist(newState.position, 0.25f);
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
        transform.position = state.position;
        
    }
    

}
