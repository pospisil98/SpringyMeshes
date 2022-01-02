using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpringyMeshIcoSphere : MonoBehaviour
{
    public int subdivisions;
    public float radius;
    
    private List<Strut> struts;
    private List<Face> faces;
    private List<Vertex> vertices;

    private List<int> triangles;
    private List<Vector3> positions;

    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    protected Mesh mesh;
    
    public Material mat;
    
    float mass = 1.0f;

    private void Awake()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = ShapeGenerator.IcoSphere.Create(subdivisions, radius);

        mesh = meshFilter.mesh;

        vertices = new List<Vertex>();
        triangles = new List<int>();
        struts = new List<Strut>();
        faces = new List<Face>();
        

        
        List<VertexHelper> vertexIndexHelper = new List<VertexHelper>();
        var originalVertices = mesh.vertices;
        for (int index = 0; index < originalVertices.Length; index++) {
            bool isNodeClose = vertexIndexHelper.Any(item => {
                float dist = Vector3.Distance(item.position, originalVertices[index]);
                return dist < 0.01f;
            });
            
            
            if (!isNodeClose) {
                vertices.Add(new Vertex(originalVertices[index], mass));
                vertexIndexHelper.Add(new VertexHelper(originalVertices[index], index, vertices.Count - 1));
            } else {
                //Debug.Log("Duplicate of node!");
            }
        }
        
        // Fix masses of vertices
        for (int i = 0; i < vertices.Count; i++) {
            vertices[i].id = i;
            vertices[i].mass = mass / vertices.Count;
        }
        
        Debug.Log("Vertex count: " + vertices.Count);
        
        // Create struts
        
        List<EdgeHelper> springsInMesh = new List<EdgeHelper>();
        List<int> editedIndexCache = new List<int>();
        for (int triangleIndex = 0; triangleIndex < mesh.triangles.Length; triangleIndex += 3) {
            int editedIndex0 = vertexIndexHelper[FindIndexOfClosestHelper(mesh.vertices[mesh.triangles[triangleIndex]], vertexIndexHelper)].editedIndex;
            int editedIndex1 = vertexIndexHelper[FindIndexOfClosestHelper(mesh.vertices[mesh.triangles[triangleIndex + 1]], vertexIndexHelper)].editedIndex;
            int editedIndex2 = vertexIndexHelper[FindIndexOfClosestHelper(mesh.vertices[mesh.triangles[triangleIndex + 2]], vertexIndexHelper)].editedIndex;
            
            editedIndexCache.Add(editedIndex0);
            editedIndexCache.Add(editedIndex1);
            editedIndexCache.Add(editedIndex2);
            
            
            if (editedIndex0 == editedIndex1 || editedIndex1 == editedIndex2 || editedIndex0 == editedIndex2) {
                Debug.Log("Dva stejný");
            }

            //Debug.Log("EditedIndex: " + editedIndex0 + "  GlobalIndex: " + mesh.triangles[triangleIndex] + "  Pos: " + mesh.vertices[mesh.triangles[triangleIndex]] + "Edited Pos:" + mesh.vertices[editedIndex0]);
            //Debug.Log("EditedIndex: " + editedIndex1 + "  GlobalIndex: " + mesh.triangles[triangleIndex + 1] + "  Pos: " + mesh.vertices[mesh.triangles[triangleIndex + 1]] + "Edited Pos:" + mesh.vertices[editedIndex1]);
            //Debug.Log("EditedIndex: " + editedIndex2 + "  GlobalIndex: " + mesh.triangles[triangleIndex + 2] + "  Pos: " + mesh.vertices[mesh.triangles[triangleIndex + 2]] + "Edited Pos:" + mesh.vertices[editedIndex2]);
        }
        
        // Create triangles
        List<HashSet<int>> triangleHelpers = new List<HashSet<int>>();
        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            HashSet<int> tri = new HashSet<int> {
                editedIndexCache[i],
                editedIndexCache[i + 1],
                editedIndexCache[i + 2]
            };

            if (!triangleHelpers.Contains(tri)) {
                triangles.Add(editedIndexCache[i]);
                triangles.Add(editedIndexCache[i + 1]);
                triangles.Add(editedIndexCache[i + 2]);

                triangleHelpers.Add(tri);
            }
        }
        
        int strutId = 0;
        for (int i = 0; i < triangles.Count; i += 3) {
            int id1 = triangles[i];
            int id2 = triangles[i + 1];
            int id3 = triangles[i + 2];

            Face face = new Face(i / 3, vertices[id1], vertices[id2], vertices[id3]);
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
                s1 = new Strut(strutId++, vertices[id1], vertices[id2]) {
                    face2 = face,
                    opposite2 = vertices[id3]
                };

                struts.Add(s1);
            } else {
                s1.face1 = face;
                s1.opposite1 = vertices[id3];
            }

            if (s2 == null) {
                s2 = new Strut(strutId++, vertices[id2], vertices[id3]) {
                    face2 = face,
                    opposite2 = vertices[id1]
                };

                struts.Add(s2);
            } else {
                s2.face1 = face;
                s2.opposite1 = vertices[id1];
            }

            if (s3 == null) {
                s3 = new Strut(strutId++, vertices[id3], vertices[id1]) {
                    face2 = face,
                    opposite2 = vertices[id2]
                };

                struts.Add(s3);
            } else {
                s3.face1 = face;
                s3.opposite1 = vertices[id2];
            }
        }
        
        float avgLength = 0.0f;
        foreach (var strut in struts) {
            avgLength += strut.restLength;
        }

        avgLength /= struts.Count;

        for (int i = 0; i < struts.Count; i++) {
            struts[i].Preprocess(avgLength);
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
        for (int i = 0; i < vertices.Count; i++) {
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

        for (int i = 0; i < faces.Count; i++) {
            faces[i].Update();
        }


        // Loop over all of the struts, adding each strut’s spring and damper forces to
        // the forces acting on the two particles it is connected to
        for (int i = 0; i < struts.Count; i++) {
            struts[i].ApplyForces();
        }


        // Loop over all of the particles, dividing the particle’s total applied force by its
        // mass to obtain its acceleration
        for (int i = 0; i < vertices.Count; i++) {
            vertices[i].Tick(Time.fixedDeltaTime, transform);
        }
    }
    
    protected virtual void Render()
    {
        mesh.Clear();

        List<Vector3> positions = new List<Vector3>();
        foreach (var vertex in vertices) {
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
        if (vertices == null) {
            return;
        }

        Gizmos.color = Color.green;

        for (int i = 0; i < vertices.Count; i++) {
            if (vertices[i].isAtRestFlag) {
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawSphere(transform.TransformPoint(vertices[i].Position), 0.05f);
        }
    }
    
    protected struct VertexHelper
    {
        public Vector3 position;
        public int globalIndex;
        public int editedIndex;

        public VertexHelper(Vector3 position, int globalIndex, int editedIndex)
        {
            this.position = position;
            this.globalIndex = globalIndex;
            this.editedIndex = editedIndex;
        }
    }
    
    protected struct EdgeHelper
    {
        public int from;
        public int to;

        public EdgeHelper(int f, int t)
        {
            this.from = f;
            this.to = t;
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is EdgeHelper))
                return false;

            EdgeHelper other = (EdgeHelper) obj;
            
            return (this.from == other.from && this.to == other.to) || (this.from == other.to && this.to == other.from);
        }
        
        public override int GetHashCode()
        {
            return this.from.GetHashCode() * 17 + this.to.GetHashCode();
        }
    }
    
    protected int FindIndexOfClosestHelper(Vector3 pos, List<VertexHelper> vertexHelpers)
    {
        int index = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < vertexHelpers.Count; i++) {
            float dist = Vector3.Distance(vertexHelpers[i].position, pos);

            if (dist < minDist) {
                minDist = dist;
                index = i;
            }
        }

        return index;
    }
}