using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBall : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject trajectoryObject;
    
    public float mass = 1.0f;
    public Vector3 position;
    public Vector3 velocity = Vector3.zero;
    public Vector3 force= Vector3.zero;
    public Vector3 windVelocity = Vector3.zero;
    public float d = 0.4f; // air resistance constant

    private float dilation = 0.5f;
    
    void Start()
    {
        position = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime * dilation;
        
        // Determine accelerations by Newton’s second law
        
        Vector3 velocityPrev = velocity;

        Vector3 acceleration = Physics.gravity + (d / mass) * (windVelocity - velocityPrev);
        
        // new-state = Integration of accelerations over timestep delta
        
        velocity = velocity + acceleration * delta;
        position = position + (velocityPrev + velocity) * 0.5f * delta;
        
        // current-state = new-state
        transform.position = position;
        
    }
    

}
