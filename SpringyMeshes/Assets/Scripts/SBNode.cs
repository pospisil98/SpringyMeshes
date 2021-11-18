using System;
using System.Collections.Generic;
using UnityEngine;


public class SBNode
{
    private float mass;
    private Vector3 position;
    
    private Vector3 force;
    private Vector3 speed;
    
    private float t = 0.0f;

    private bool isFixed = false;
    
    private List<SBSDampedSpring> springs = new List<SBSDampedSpring>();

    public SBNode(Vector3 position, float mass) {
        this.position = position;
        this.mass = mass;
        this.force = Vector3.zero;
        this.speed = Vector3.zero;
    }

    public void Tick(float deltaTime) {
       /*
        if (isFixed) {
            speed = Vector3.zero;
        } else {
            speed += deltaTime * force / mass; 
            position += deltaTime * speed;
        }
        */
       t += deltaTime;

       if (isFixed) {
            
       } else
       {
           float phi = -(float)Math.Atan2(Math.Sqrt(mass), position.y);
           float c = (float) Math.Sqrt(position.y * position.y + mass);

           position.y = c * (float)Math.Cos(Math.Sqrt(1 / mass) * t + phi);
       }
    }

    public void AddForce(Vector3 force)
    {
        this.force += force;
    }

    public float Mass
    {
        get => mass;
        set => mass = value;
    }

    public Vector3 Position
    {
        get => position;
        set => position = value;
    }

    public Vector3 Force
    {
        get => force;
        set => force = value;
    }

    public Vector3 Speed
    {
        get => speed;
        set => speed = value;
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
