using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SpringyMesh : MonoBehaviour
{
    // [Range(0, 50)] public float k = 4.0f * (float) (Math.PI * Math.PI);
    // [Range(0, 20)] public float d = 0.8f * (float) Math.PI;

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



        float mass = 4.0f;
        
        // tohle je pro tetrahedronek: 
        // int vertexCount = 4;
        // float T = 0.5f;
        // float P = 0.25f;        
        
        // tohle je pro krylicku:
        int vertexCount = 8;
        float T = 5.0f;
        float P = 2.5f;

        float d = 2.0f * mass / T;
        float k = 4.0f * Mathf.PI * Mathf.PI * (mass / vertexCount) / (P * P);
        
        // Debug.Log(k + " " + d);

         // cube geometry
         vertices = new List<Vertex>
         {
             new Vertex(new Vector3(0, 0, 0), mass / vertexCount),
             new Vertex(new Vector3(1, 0, 0), mass / vertexCount),
             new Vertex(new Vector3(1, 1, 0), mass / vertexCount),
             new Vertex(new Vector3(0, 1, 0), mass / vertexCount),
             new Vertex(new Vector3(0, 1, 1), mass / vertexCount),
             new Vertex(new Vector3(1, 1, 1), mass / vertexCount),
             new Vertex(new Vector3(1, 0, 1), mass / vertexCount),
             new Vertex(new Vector3(0, 0, 1), mass / vertexCount),
         };
         
         triangles = new List<int>
         {
             0, 2, 1, // f1 //face front
             0, 3, 2, // f2
             2, 3, 4, // f3 //face top
             2, 4, 5, // f4
             1, 2, 5, // f5 //face right
             1, 5, 6, // f6
             0, 7, 4, // f7 //face left
             0, 4, 3, // f8
             5, 4, 7, // f9 //face back
             5, 7, 6, // f10
             0, 6, 7, // f11 //face bottom
             0, 1, 6  // f12
         };
         
        // tetrahedron geometry
        // vertices = new List<Vertex>
        // {
        //     new Vertex(new Vector3(0, 0, 0), mass / vertexCount),
        //     new Vertex(new Vector3(1, 0, 0), mass / vertexCount),
        //     new Vertex(new Vector3(0, 0, 1), mass / vertexCount),
        //     new Vertex(new Vector3(0, 1, 0), mass / vertexCount),
        // };
        //
        // triangles = new List<int>
        // {
        //     0, 1, 2, // floor
        //     0, 3, 1, // XY
        //     0, 2, 3, // ZY
        //     1, 3, 2  
        // };
        
        
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i].id = i;
        }

        int strutId = 0;
        for (int i = 0; i < triangles.Count; i += 3)
        {
            int id1 = triangles[i];
            int id2 = triangles[i + 1];
            int id3 = triangles[i + 2];

            Face face = new Face(i / 3, vertices[id1], vertices[id2], vertices[id3]);
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
                s1 = new Strut(strutId++, k, d, vertices[id1], vertices[id2])
                {
                    face2 = face,
                    opposite2 = vertices[id3]
                };

                struts.Add(s1);
            }
            else
            {
                s1.face1 = face;
                s1.opposite1 = vertices[id3];
                s1.Preprocess();
            }

            if (s2 == null)
            {
                s2 = new Strut(strutId++, k, d, vertices[id2], vertices[id3])
                {
                    face2 = face,
                    opposite2 = vertices[id1]
                };

                struts.Add(s2);
            }
            else
            {
                s2.face1 = face;
                s2.opposite1 = vertices[id1];
                s2.Preprocess();
            }

            if (s3 == null)
            {
                s3 = new Strut(strutId++, k, d, vertices[id3], vertices[id1])
                {
                    face2 = face,
                    opposite2 = vertices[id2]
                };

                struts.Add(s3);
            }
            else
            {
                s3.face1 = face;
                s3.opposite1 = vertices[id2];
                s3.Preprocess();
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
        
        // float dd = 0.1f;
        // //air drag
        // for (int i = 0; i < vertices.Count; i++)
        // {
        //     vertices[i].AddForce(- dd  * vertices[i].Velocity);
        // }

        //wind
        // float u = UnityEngine.Random.value;
        // float v = UnityEngine.Random.value;
        // for (int i = 0; i < vertices.Count; i++)
        // {
        //     vertices[i].AddForce(dd * (u * Vector3.right + v * Vector3.forward) * 4.0f);
        // }

        for (int i = 0; i < faces.Count; i++)
        {
            faces[i].Update();
        }


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
        Render();
    }

    protected virtual void Render()
    {
        mesh.Clear();

        List<Vector3> positions = new List<Vector3>();
        foreach (var vertex in vertices)
        {
            positions.Add(vertex.Position);
        }

        // needs local coordiantes
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