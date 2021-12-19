using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SpringyMesh : MonoBehaviour
{
    [Range(0, 50)] public float k = 4.0f * (float) (Math.PI * Math.PI);
    [Range(0, 20)] public float d = 0.8f * (float) Math.PI;

    private List<Strut> struts;
    private List<Face> faces;
    private List<Vertex> vertices;

    private List<int> triangles;
    private List<Vector3> positions;

    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    protected Mesh mesh;

    public Material mat;

    private void Awake()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        struts = new List<Strut>();
        faces = new List<Face>();

        vertices = new List<Vertex>
        {
            new Vertex(new Vector3(0, 0, 0), 1.0f),
            new Vertex(new Vector3(1, 0, 0), 1.0f),
            new Vertex(new Vector3(1, 1, 0), 1.0f),
            new Vertex(new Vector3(0, 1, 0), 1.0f),
            new Vertex(new Vector3(0, 1, 1), 1.0f),
            new Vertex(new Vector3(1, 1, 1), 1.0f),
            new Vertex(new Vector3(1, 0, 1), 1.0f),
            new Vertex(new Vector3(0, 0, 1), 1.0f),
        };

        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i].id = i;
        }


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


        for (int i = 0; i < triangles.Count; i += 3)
        {
            int id1 = triangles[i];
            int id2 = triangles[i + 1];
            int id3 = triangles[i + 2];

            Face face = new Face(0, 0, 0);
            faces.Add(face);

            Strut s1 = null;
            Strut s2 = null;
            Strut s3 = null;
            foreach (Strut strut in struts)
            {
                if (strut.IsSame(id1, id2))
                {
                    s1 = strut;
                }

                if (strut.IsSame(id2, id3))
                {
                    s2 = strut;
                }

                if (strut.IsSame(id3, id1))
                {
                    s3 = strut;
                }
            }


            if (s1 == null)
            {
                s1 = new Strut(k, d, vertices[id1], vertices[id2])
                {
                    f1 = face,
                    opposite1 = vertices[id3]
                };

                struts.Add(s1);
            }
            else
            {
                s1.f2 = face;
                s1.opposite2 = vertices[id3];
            }

            if (s2 == null)
            {
                s2 = new Strut(k, d, vertices[id2], vertices[id3])
                {
                    f1 = face,
                    opposite1 = vertices[id1]
                };

                struts.Add(s2);
            }
            else
            {
                s2.f2 = face;
                s2.opposite2 = vertices[id1];
            }

            if (s3 == null)
            {
                s3 = new Strut(k, d, vertices[id3], vertices[id1])
                {
                    f1 = face,
                    opposite1 = vertices[id2]
                };

                struts.Add(s3);
            }
            else
            {
                s3.f2 = face;
                s3.opposite2 = vertices[id2];
            }
        }
    }

    private void Update()
    {
        Render();
    }

    private void FixedUpdate()
    {
        // Loop over all of the particles, setting each particle’s force to the accumulation
        // of all external forces acting directly on each particle, such as air drag, friction,
        // or gravity
        // gravity
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i].AddForce(transform.InverseTransformVector(Physics.gravity * vertices[i].mass));
        }

        //float dd = 1.0f;
        // air drag
        // for (int i = 0; i < particles.Count; i++)
        // {
        //     particles[i].AddForce(- dd  * particles[i].Velocity);
        // }

        // wind
        // float u = UnityEngine.Random.value;
        // float v = UnityEngine.Random.value;
        // for (int i = 0; i < particles.Count; i++)
        // {
        //     particles[i].AddForce(dd * (u * Vector3.right + v * Vector3.forward) * 4.0f);
        // }


        // Loop over all of the struts, adding each strut’s spring and damper forces to
        // the forces acting on the two particles it is connected to
        for (int i = 0; i < struts.Count; i++)
        {
            struts[i].ApplyForces();
        }


        // Loop over all of the particles, dividing the particle’s total applied force by its
        // mass to obtain its acceleration
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i].Tick(Time.fixedDeltaTime, transform);
        }
    }

    protected virtual void Render()
    {
        mesh.Clear();

        List<Vector3> positions = new List<Vector3>();
        foreach (var vertex in vertices)
        {
            positions.Add(vertex.Position);
        }

        mesh.vertices = positions.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }

        Gizmos.color = Color.green;

        for (int i = 0; i < vertices.Count; i++)
        {
            if (vertices[i].isAtRestFlag) {
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawSphere(transform.TransformPoint(vertices[i].Position), 0.05f);
        }
    }
}