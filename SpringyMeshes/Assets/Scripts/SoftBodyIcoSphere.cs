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
        meshFilter.mesh = ShapeGenerator.IcoSphere.Create(subdivisions, radius);
        //meshFilter.mesh = ShapeGenerator.IcoSphereUnity.Create(subdivisions, radius);
        mesh = meshFilter.mesh;


        particles = new List<SBNode>();
        dampedSprings = new List<SBSDampedSpring>();
        triangles = new List<int>();

        List<VertexHelper> vertexIndexHelper = new List<VertexHelper>();
        var originalVertices = mesh.vertices;
        for (int index = 0; index < originalVertices.Length; index++) {
            bool isNodeClose = vertexIndexHelper.Any(item => {
                float dist = Vector3.Distance(item.position, originalVertices[index]);
                return dist < 0.01f;
            });
            
            
            if (!isNodeClose) {
                particles.Add(new SBNode(originalVertices[index], 0.1f));
                vertexIndexHelper.Add(new VertexHelper(originalVertices[index], index, particles.Count - 1));
            } else {
                //Debug.Log("Duplicate of node!");
            }
        }
        Debug.Log("Particle count: " + particles.Count);
        
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
                Debug.Log("Sva stejný cigáne");
            }

            //Debug.Log("EditedIndex: " + editedIndex0 + "  GlobalIndex: " + mesh.triangles[triangleIndex] + "  Pos: " + mesh.vertices[mesh.triangles[triangleIndex]] + "Edited Pos:" + mesh.vertices[editedIndex0]);
            //Debug.Log("EditedIndex: " + editedIndex1 + "  GlobalIndex: " + mesh.triangles[triangleIndex + 1] + "  Pos: " + mesh.vertices[mesh.triangles[triangleIndex + 1]] + "Edited Pos:" + mesh.vertices[editedIndex1]);
            //Debug.Log("EditedIndex: " + editedIndex2 + "  GlobalIndex: " + mesh.triangles[triangleIndex + 2] + "  Pos: " + mesh.vertices[mesh.triangles[triangleIndex + 2]] + "Edited Pos:" + mesh.vertices[editedIndex2]);
            
            EdgeHelper edgeHelper = new EdgeHelper(editedIndex0, editedIndex1);
            if (!springsInMesh.Any(x => x.Equals(edgeHelper))) {
                dampedSprings.Add(new SBSDampedSpring(particles[editedIndex0],particles[editedIndex1], k, d));
                springsInMesh.Add(edgeHelper);
            }

            edgeHelper = new EdgeHelper(editedIndex1, editedIndex2);
            if (!springsInMesh.Any(x => x.Equals(edgeHelper))) {
                dampedSprings.Add(new SBSDampedSpring(particles[editedIndex1], particles[editedIndex2], k, d));
                springsInMesh.Add(edgeHelper);
            }

            edgeHelper = new EdgeHelper(editedIndex2, editedIndex0);
            if (!springsInMesh.Any(x => x.Equals(edgeHelper))) {
                dampedSprings.Add(new SBSDampedSpring(particles[editedIndex2],particles[editedIndex0], k, d));
                springsInMesh.Add(edgeHelper);
            }
        }

        List<HashSet<int>> triangleHelpers = new List<HashSet<int>>();
        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            HashSet<int> tri = new HashSet<int>();
            tri.Add(editedIndexCache[i]);
            tri.Add(editedIndexCache[i+1]);
            tri.Add(editedIndexCache[i+2]);

            if (!triangleHelpers.Contains(tri)) {
                triangles.Add(editedIndexCache[i]);
                triangles.Add(editedIndexCache[i+1]);
                triangles.Add(editedIndexCache[i+2]);
                
                triangleHelpers.Add(tri);
            }
        }
        
        Debug.Log("Spring count: " + dampedSprings.Count);
        Debug.Log("Triangle vertind count: " + triangles.Count);
        
        
        particles.Add(new SBNode(Vector3.zero, 1f));
        for (int i = 0; i < particles.Count - 1; i++) {
            // dampedSprings.Add(new SBSDampedSpring(particles[i], particles[particles.Count - 1], 20f, 0.6f));
            dampedSprings.Add(new SBSDampedSpring(particles[i], particles[particles.Count - 1], k, d));
        }
    }

    

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}