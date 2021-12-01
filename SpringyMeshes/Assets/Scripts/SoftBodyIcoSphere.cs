using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class SoftBodyIcoSphere : SoftBody
{
    public int subdivisions;
    public float radius;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void InitObject()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = ShapeGenerator.IcoSphereUnity.Create(subdivisions, radius);
        mesh = meshFilter.mesh;

        particles = new List<SBNode>();
        dampedSprings = new List<SBSDampedSpring>();
        triangles = new List<int>();

        Dictionary<Vector3, VertexHelper> vertexIndexDict = new Dictionary<Vector3, VertexHelper>(new VertexComparer());
        for (int index = 0; index < mesh.vertices.Length; index++) {
            if (!vertexIndexDict.ContainsKey(mesh.vertices[index])) {
                particles.Add(new SBNode(mesh.vertices[index], 0.1f));
                vertexIndexDict.Add(mesh.vertices[index], new VertexHelper(index, particles.Count - 1));
            }
        }

        HashSet<EdgeHelper> springsInMesh = new HashSet<EdgeHelper>(new EdgeHelperComparer());
        for (int triangleIndex = 0; triangleIndex < mesh.triangles.Length; triangleIndex += 3) {
            EdgeHelper edgeHelper = new EdgeHelper(triangleIndex, triangleIndex + 1);
            if (!springsInMesh.Contains(edgeHelper)) {
                dampedSprings.Add(new SBSDampedSpring(particles[vertexIndexDict[mesh.vertices[mesh.triangles[triangleIndex]]].editedIndex],
                    particles[vertexIndexDict[mesh.vertices[mesh.triangles[triangleIndex + 1]]].editedIndex]));
                springsInMesh.Add(edgeHelper);
            }

            edgeHelper = new EdgeHelper(triangleIndex + 1, triangleIndex + 2);
            if (!springsInMesh.Contains(edgeHelper)) {
                dampedSprings.Add(new SBSDampedSpring(particles[vertexIndexDict[mesh.vertices[mesh.triangles[triangleIndex + 1]]].editedIndex],
                    particles[vertexIndexDict[mesh.vertices[mesh.triangles[triangleIndex + 2]]].editedIndex]));
                springsInMesh.Add(edgeHelper);
            }

            edgeHelper = new EdgeHelper(triangleIndex + 2, triangleIndex);
            if (!springsInMesh.Contains(edgeHelper)) {
                dampedSprings.Add(new SBSDampedSpring(particles[vertexIndexDict[mesh.vertices[mesh.triangles[triangleIndex]]].editedIndex],
                    particles[vertexIndexDict[mesh.vertices[mesh.triangles[triangleIndex + 2]]].editedIndex]));
                springsInMesh.Add(edgeHelper);
            }
        }

        for (int i = 0; i < mesh.triangles.Length; i++) {
            triangles.Add(vertexIndexDict[mesh.vertices[mesh.triangles[i]]].editedIndex);
        } 
        
        particles.Add(new SBNode(Vector3.zero, 1f));
        for (int i = 0; i < particles.Count - 1; i++) {
            dampedSprings.Add(new SBSDampedSpring(particles[i], particles[particles.Count - 1]));
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    private struct EdgeHelper
    {
        public int from;
        public int to;

        public EdgeHelper(int f, int t)
        {
            this.from = f;
            this.to = t;
        }
    }

    private class EdgeHelperComparer : IEqualityComparer<EdgeHelper>
    {
        public bool Equals(EdgeHelper one, EdgeHelper two)
        {
            return (one.from == two.from && one.to == two.to) || (one.from == two.to && one.to == two.from);
        }

        public int GetHashCode(EdgeHelper item)
        {
            return item.from.GetHashCode() * 17 + item.to.GetHashCode();
        }
    }
    
    private class VertexComparer : IEqualityComparer<Vector3>
    {
        public bool Equals(Vector3 one, Vector3 two)
        {
            return Vector3.Distance(one, two) < 1f;
        }

        public int GetHashCode(Vector3 item)
        {
            return item.GetHashCode();
        }
    }

    private struct VertexHelper
    {
        public int globalIndex;
        public int editedIndex;

        public VertexHelper(int globalIndex, int editedIndex)
        {
            this.globalIndex = globalIndex;
            this.editedIndex = editedIndex;
        }
    }
}