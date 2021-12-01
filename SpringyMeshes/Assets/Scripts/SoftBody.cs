using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoftBody : MonoBehaviour
{
    public bool drawGizmos = false;
    public Material mat;

    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    protected Mesh mesh;

    protected List<SBNode> particles;
    protected List<SBSDampedSpring> dampedSprings;
    protected List<int> triangles;

    protected virtual void OnEnable()
    {
        InitObject();
    }

    protected virtual void Start()
    {
        Render();
    }

    protected virtual void Update()
    {
        Render();
    }

    protected virtual void FixedUpdate()
    {
        // Loop over all of the particles, setting each particle’s force to the accumulation
        // of all external forces acting directly on each particle, such as air drag, friction,
        // or gravity
        // gravity
        for (int i = 0; i < particles.Count; i++) {
            particles[i].AddForce(transform.InverseTransformVector(Physics.gravity * particles[i].Mass));
        }

        float d = 1.0f;
        // air drag
        // for (int i = 0; i < particles.Count; i++)
        // {
        //     particles[i].AddForce(- d  * particles[i].Velocity);
        // }

        // wind
        // float u = UnityEngine.Random.value;
        // float v = UnityEngine.Random.value;
        // for (int i = 0; i < particles.Count; i++)
        // {
        //     particles[i].AddForce(d * (u * Vector3.right + v * Vector3.forward) * 4.0f);
        // }


        // Loop over all of the struts, adding each strut’s spring and damper forces to
        // the forces acting on the two particles it is connected to
        for (int i = 0; i < dampedSprings.Count; i++) {
            dampedSprings[i].ApplyForces();
        }


        // Loop over all of the particles, dividing the particle’s total applied force by its
        // mass to obtain its acceleration
        for (int i = 0; i < particles.Count; i++) {
            particles[i].Tick(Time.fixedDeltaTime, transform);
        }
    }

    protected abstract void InitObject();

    protected virtual void Render()
    {
        mesh.Clear();

        List<Vector3> positions = new List<Vector3>();
        foreach (var node in particles) {
            positions.Add(node.Position);
        }

        mesh.vertices = positions.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.Optimize();
        mesh.RecalculateNormals();
    }

    protected virtual void OnDrawGizmos()
    {
        if (drawGizmos) {
            if (particles != null) {
                for (int i = 0; i < particles.Count; i++) {
                    if (particles[i].isAtRestFlag) {
                        Gizmos.color = Color.green;
                    } else {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawSphere(transform.TransformPoint(particles[i].Position), 0.05f);
                }

                for (int i = 0; i < dampedSprings.Count; i++) {
                    Gizmos.color = Color.cyan;
                    Vector3 dir = transform.TransformVector(dampedSprings[i].GetEndPosition()) -
                                  transform.TransformVector(dampedSprings[i].GetStartPosition());
                    //DrawArrow.ForGizmo(transform.TransformVector(dampedSprings[i].GetStartPosition()), dir);
                    Gizmos.DrawLine(transform.TransformPoint(dampedSprings[i].GetStartPosition()), transform.TransformPoint(dampedSprings[i].GetEndPosition()));
                }
            }
        }
    }
}