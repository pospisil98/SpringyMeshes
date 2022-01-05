using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SpringyMeshBase : MonoBehaviour
{
    public bool drawGizmos = true;
    public float mass;
    public Material mat;
    
    protected List<Vertex> vertices;
    protected List<Strut> struts;
    protected List<Face> faces;

    protected List<int> triangles;

    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    protected Mesh mesh;
    
    protected virtual void Render()
    {
        mesh.Clear();
        
        // Update geometry
        List<Vector3> positions = new List<Vector3>();
        foreach (Vertex vertex in vertices) {
            positions.Add(vertex.Position);
        }
        
        mesh.vertices = positions.ToArray();
        mesh.triangles = triangles.ToArray();

        // Stuff recommended from docs
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
    }

    protected virtual void ProcessInput()
    {
        // User input
        float acceleration = 100.0f;
        if(Input.GetKey(KeyCode.W)){
            Debug.Log("W");
            for (int i = 0; i < vertices.Count; i++) {
                vertices[i].AddForce(transform.InverseTransformVector(acceleration * Vector3.forward * vertices[i].mass));
            }
        }
        if(Input.GetKey(KeyCode.S)){
            Debug.Log("S");
            for (int i = 0; i < vertices.Count; i++) {
                vertices[i].AddForce(transform.InverseTransformVector( - acceleration * Vector3.forward * vertices[i].mass));
            }
        }
        if(Input.GetKey(KeyCode.D)){
            Debug.Log("D");
            for (int i = 0; i < vertices.Count; i++) {
                vertices[i].AddForce(transform.InverseTransformVector(acceleration * Vector3.right * vertices[i].mass));
            }
        }
        if(Input.GetKey(KeyCode.A)){
            Debug.Log("A");
            for (int i = 0; i < vertices.Count; i++) {
                vertices[i].AddForce(transform.InverseTransformVector(-acceleration * Vector3.right * vertices[i].mass));
            }
        }
        if(Input.GetKey(KeyCode.Space)){
            Debug.Log("Space");
            for (int i = 0; i < vertices.Count; i++) {
                vertices[i].AddForce(transform.InverseTransformVector(5.0f * acceleration * Vector3.up * vertices[i].mass));
            }
        }
    }

    protected float GetAverageStrutLength()
    {
        return struts.Sum(strut => strut.restLength) / struts.Count;
    }

    protected void SetupVerticesMassBasedOnAnglesAndAreas(List<HashSet<int>> neighbourFaces)
    {
        float totalArea = faces.Sum(f => f.initialArea);
        for (int i = 0; i < vertices.Count; i++) {
            float vertexMass = 0.0f;
            HashSet<int> n = neighbourFaces[i];
            foreach (int neiFaceIndex in n) {
                vertexMass += (faces[neiFaceIndex].GetAngleByVertex(vertices[i]) / 180.0f) * (faces[neiFaceIndex].initialArea / totalArea) * mass;
            }

            vertices[i].mass = vertexMass;
        }
    }

    protected abstract void InitMeshStuff();

    protected virtual void InitObject()
    {
        List<VertexHelper> vertexIndexHelper = new List<VertexHelper>();
        var originalVertices = mesh.vertices;
        for (int index = 0; index < originalVertices.Length; index++) {
            bool isNodeClose = vertexIndexHelper.Any(item => {
                float dist = Vector3.Distance(item.position, originalVertices[index]);
                return dist < 0.01f;
            });
            
            if (!isNodeClose) {
                vertices.Add(new Vertex(originalVertices[index], mass));
                vertexIndexHelper.Add(new VertexHelper(originalVertices[index], vertices.Count - 1));
            }
        }
        
        // Setup vertices ids
        for (int i = 0; i < vertices.Count; i++) {
            vertices[i].id = i;
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
                Debug.LogError("Two vertices in triangle are same.");
            }
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
        
        List<HashSet<int>> neighbourFaces = new List<HashSet<int>>();
        for (int i = 0; i < vertices.Count; i++) {
            neighbourFaces.Add(new HashSet<int>());
        }
        
        int strutId = 0;
        for (int i = 0; i < triangles.Count; i += 3) {
            int id1 = triangles[i];
            int id2 = triangles[i + 1];
            int id3 = triangles[i + 2];

            Face face = new Face(i / 3, vertices[id1], vertices[id2], vertices[id3]);
            faces.Add(face);

            // Add index of neighbour face in helper list
            neighbourFaces[id1].Add(i / 3);
            neighbourFaces[id2].Add(i / 3);
            neighbourFaces[id3].Add(i / 3);

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
        
        // Setup masses based on angles and triangle areas
        SetupVerticesMassBasedOnAnglesAndAreas(neighbourFaces);
        
        // Calculate average strut length
        float avgLength = GetAverageStrutLength();
        
        // Preprocess struts
        for (int i = 0; i < struts.Count; i++) {
            struts[i].Preprocess(avgLength);
        }
    }

    protected virtual void OnEnable()
    {
        vertices = new List<Vertex>();
        triangles = new List<int>();
        struts = new List<Strut>();
        faces = new List<Face>();
        
        InitMeshStuff();
        InitObject();
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
        
        // Gravity
        for (int i = 0; i < vertices.Count; i++) {
            vertices[i].AddForce(transform.InverseTransformVector(Physics.gravity * vertices[i].mass));
        }

        
        // Air drag
        // float dd = 0.1f;
        // for (int i = 0; i < vertices.Count; i++)
        // {
        //     vertices[i].AddForce(- dd  * vertices[i].Velocity);
        // }

        // Wind
        // float u = UnityEngine.Random.value;
        // float v = UnityEngine.Random.value;
        // for (int i = 0; i < vertices.Count; i++)
        // {
        //     vertices[i].AddForce(dd * (u * Vector3.right + v * Vector3.forward) * 4.0f);
        // }

        // Update faces (angles, area, etc.)
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
    
    protected virtual void OnDrawGizmos()
    {
        if (vertices == null || drawGizmos == false) {
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
        public int editedIndex;

        public VertexHelper(Vector3 position, int editedIndex)
        {
            this.position = position;
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