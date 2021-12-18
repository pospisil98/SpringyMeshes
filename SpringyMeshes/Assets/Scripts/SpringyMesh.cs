using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SpringyMesh : MonoBehaviour
{
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
        
        vertices = new List<Vertex> {
            new Vertex(new Vector3(0, 0, 0), 1.0f),
            new Vertex(new Vector3(1, 0, 0), 1.0f),
            new Vertex(new Vector3(1, 1, 0), 1.0f),
            new Vertex(new Vector3(0, 1, 0), 1.0f),
            new Vertex(new Vector3(0, 1, 1), 1.0f),
            new Vertex(new Vector3(1, 1, 1), 1.0f),
            new Vertex(new Vector3(1, 0, 1), 1.0f),
            new Vertex(new Vector3(0, 0, 1), 1.0f),
        };

        for (int i = 0; i < vertices.Count; i++) {
            vertices[i].id = i;
        }


        triangles = new List<int> {
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


        for (int i = 0; i < triangles.Count; i += 3) {
            int id1 = triangles[i];
            int id2 = triangles[i + 1];
            int id3 = triangles[i + 2];

            Face face = new Face(0, 0, 0);
            faces.Add(face);

            Strut s1 = null;
            Strut s2 = null;
            Strut s3 = null;
            foreach (Strut strut in struts) {
                if (strut.IsSame(id1, id2)) {
                    s1 = strut;
                }

                if (strut.IsSame(id2, id3)) {
                    s2 = strut;
                }

                if (strut.IsSame(id3, id1)) {
                    s3 = strut;
                }
            }


            if (s1 == null) {
                s1 = new Strut(1, 1, vertices[id1], vertices[id2]) {
                    f1 = face,
                    opposite1 = vertices[id3]
                };

                struts.Add(s1);
            } else {
                s1.f2 = face;
                s1.opposite2 = vertices[id3];
            }

            if (s2 == null) {
                s2 = new Strut(1, 1, vertices[id2], vertices[id3]) {
                    f1 = face,
                    opposite1 = vertices[id1]
                };

                struts.Add(s2);
            } else {
                s2.f2 = face;
                s2.opposite2 = vertices[id1];
            }

            if (s3 == null) {
                s3 = new Strut(1, 1, vertices[id3], vertices[id1]) {
                    f1 = face,
                    opposite1 = vertices[id2]
                };

                struts.Add(s3);
            } else {
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
         // throw new NotImplementedException();
    }

    protected virtual void Render()
    {
        mesh.Clear();

        List<Vector3> positions = new List<Vector3>();
        foreach (var node in vertices) {
            positions.Add(node.position);
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
        if (vertices == null) {
            return;
        }

        Gizmos.color = Color.green;
        
        for (int i = 0; i < vertices.Count; i++) {
                Gizmos.DrawSphere(transform.TransformPoint(vertices[i].position), 0.05f);
        }
    }


    public class Strut
    {
        public double k;
        public double d;
        public double l0;
        public double kTheta;

        public Vertex from;
        public Vertex to;

        public Face f1;
        public Face f2;

        public Vertex opposite1;
        public Vertex opposite2;

        public Strut(double k, double d, Vertex from, Vertex to)
        {
            this.k = k;
            this.d = d;

            this.from = from;
            this.to = to;
            
            this.l0 = Vector3.Distance(from.position, to.position);
            //TODO: fix
            this.kTheta = 1;
        }

        public bool IsSame(int id1, int id2)
        {
            return (from.id == id1 && to.id == id2) || (@from.id == id2 && to.id == id1);
        }
    }

    public class Face
    {
        public double angle1;
        public double angle2;
        public double angle3;

        public Strut s1;
        public Strut s2;
        public Strut s3;

        public Face(double angle1, double angle2, double angle3)
        {
            this.angle1 = angle1;
            this.angle2 = angle2;
            this.angle3 = angle3;
        }
    }

    public class Vertex
    {
        public int id;

        public Vector3 position;
        public Vector3 velocity;
        public Vector3 force;

        public double mass;

        public Vertex(Vector3 position, double mass)
        {
            this.position = position;
            this.mass = mass;

            this.force = Vector3.zero;
            this.velocity = Vector3.zero;
        }
    }
}