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
       
        if (isFixed) {
            speed = Vector3.zero;
        } else {
            speed += deltaTime * force / mass; 
            position += deltaTime * speed;
        }
        
       
       // spring-mass-damper
       // t += deltaTime;
       // float v = 10;
       // float omega = (float)(2.0 * Math.PI);
       // float k = 4 * (float)(Math.PI * Math.PI);
       // float d = 0.1f * (float) (Math.PI);
       // float Pn = 2.0f * (float) ((Math.PI) * Math.Sqrt(mass / k));
       // float c = (float) Math.Sqrt(position.y * position.y + v * mass / k);
       // float vrtulka = d / (2.0f * (float) (Math.Sqrt(k * mass)));
       // float phi = -(float)Math.Atan2(Math.Sqrt(mass) * v, position.y * Math.Sqrt(k));
       // if (!isFixed)
       // {
       //     position.y = c * (float) Math.Exp(-vrtulka * omega * t) *
       //                  (float) (Math.Cos(omega * Math.Sqrt(1.0 - vrtulka * vrtulka) * t + phi));
       // }

       // damperless
       // float v = 1;
       // float k = 4 * (float)(Math.PI * Math.PI);
       //
       // if (!isFixed) {
       // {
       //     float phi = -(float)Math.Atan2(Math.Sqrt(mass) * v, position.y * Math.Sqrt(k));
       //     float c = (float) Math.Sqrt(position.y * position.y + v * mass / k);
       //
       //     position.y = c * (float)Math.Cos(Math.Sqrt(k / mass) * t + phi);
       // }
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
