using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Random = System.Random;

public class SoftBody : MonoBehaviour
{
    public bool drawGizmos = false;
    public Material mat;

    public Box box;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private List<SBNode> particles;
    private List<SBSDampedSpring> dampedSprings;
    private List<int> triangles;

    private void OnEnable()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        particles = new List<SBNode>
        {
            new SBNode( new Vector3(0, 0, 0), 1.0f),
            new SBNode( new Vector3(1, 0, 0), 1.0f),
            new SBNode( new Vector3(1, 1, 0), 1.0f),
            new SBNode( new Vector3(0, 1, 0), 1.0f),
            new SBNode( new Vector3(0, 1, 1), 1.0f),
            new SBNode( new Vector3(1, 1, 1), 1.0f),
            new SBNode( new Vector3(1, 0, 1), 1.0f),
            new SBNode( new Vector3(0, 0, 1), 1.0f),
        };
        //
        // particles[2].IsFixed = true;
        // particles[3].IsFixed = true;
        // particles[4].IsFixed = true;
        // particles[5].IsFixed = true;

        dampedSprings = new List<SBSDampedSpring>
        {
            // vertical
            new SBSDampedSpring(particles[1], particles[2]),
            new SBSDampedSpring(particles[3], particles[0]),
            new SBSDampedSpring(particles[5], particles[6]),
            new SBSDampedSpring(particles[4], particles[7]),
            
            // vertical diagonal
            new SBSDampedSpring(particles[0], particles[2]),
            new SBSDampedSpring(particles[1], particles[3]),
            new SBSDampedSpring(particles[1], particles[5]),
            new SBSDampedSpring(particles[2], particles[6]),
            new SBSDampedSpring(particles[4], particles[6]),
            new SBSDampedSpring(particles[5], particles[7]),
            new SBSDampedSpring(particles[0], particles[4]),
            new SBSDampedSpring(particles[3], particles[7]),
            
            

            // horizontal bottom
            new SBSDampedSpring(particles[0], particles[1]),
             new SBSDampedSpring(particles[1], particles[6]),
            new SBSDampedSpring(particles[6], particles[7]),
             new SBSDampedSpring(particles[0], particles[7]),
             
             // horizontal bottom diagonal
             new SBSDampedSpring(particles[0], particles[6]),
             new SBSDampedSpring(particles[1], particles[7]),
            
            // horizontal top
            new SBSDampedSpring(particles[2], particles[3]),
             new SBSDampedSpring(particles[2], particles[5]),
             new SBSDampedSpring(particles[3], particles[4]),
            new SBSDampedSpring(particles[4], particles[5]),
            
            // horizontal top diagonal
            new SBSDampedSpring(particles[2], particles[4]),
            new SBSDampedSpring(particles[3], particles[5]),
            
            
            new SBSDampedSpring(particles[0], particles[5]),
            new SBSDampedSpring(particles[1], particles[4]),
            new SBSDampedSpring(particles[3], particles[6]),
            new SBSDampedSpring(particles[2], particles[7]),
            
            
        };

        triangles = new List<int>
        {
            0, 2, 1, //face front
            0, 3, 2,
            2, 3, 4, //face top
            2, 4, 5,
            1, 2, 5, //face right
            1, 5, 6,
            0, 7, 4, //face left
            0, 4, 3,
            5, 4, 7, //face back
            5, 7, 6,
            0, 6, 7, //face bottom
            0, 1, 6
        };
    }

    void Start()
    {
        RenderCube();
    }

    private void Update()
    {
        RenderCube();
    }

    private void FixedUpdate()
    {
        // Loop over all of the particles, setting each particle’s force to the accumulation
        // of all external forces acting directly on each particle, such as air drag, friction,
        // or gravity
        // gravity
        for (int i = 0; i < particles.Count; i++)
        {
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
        for (int i = 0; i < dampedSprings.Count; i++)
        {
            dampedSprings[i].ApplyForces();
        }

        
        // Loop over all of the particles, dividing the particle’s total applied force by its
        // mass to obtain its acceleration
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Tick(Time.fixedDeltaTime, transform, box);
        }

    }

    private void RenderCube()
    {
        mesh.Clear();

        List<Vector3> positions = new List<Vector3>();
        foreach (var node in particles)
        {
            positions.Add(node.Position);
        }

        mesh.vertices = positions.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.Optimize();
        mesh.RecalculateNormals();
    }


    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (particles != null)
            {
                for (int i = 0; i < particles.Count; i++)
                {
                    if (particles[i].isAtRestFlag) {
                        Gizmos.color = Color.green;
                    } else {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawSphere(transform.TransformPoint(particles[i].Position), 0.05f);
                }
            }
        }
    }
}
