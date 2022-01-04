using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public bool drawGizmos = false;
    public Material mat;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<int> triangleIndices;
    public List<Triangle> triangles;

    private void OnEnable()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        
        // vertices = new List<Vector3>
        // {
        //     new Vector3(0, 0, 0),
        //     new Vector3(0, 0, 1),
        //     new Vector3(1, 0, 0),
        // };
        //
        //
        // triangleIndices = new List<int>
        // {
        //     0, 1, 2,
        // };
        
        vertices = new List<Vector3>
        {
            new Vector3(-1, 0.5f, 0),
            new Vector3(-1, 0.5f, 1),
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(2, -0.5f, 0),
            new Vector3(2, -0.5f, 1),
        };
        
        
        triangleIndices = new List<int>
        {
            0, 1, 2,
            2, 1, 3,
            4, 2, 3,
            4, 3, 5,
            6, 4, 5,
            6, 5, 7,
        };
        
        // vertices = new List<Vector3>
        // {
        //     new Vector3(0, 0, 0),
        //     new Vector3(4, 0, 0),
        //     new Vector3(4, 1, 0),
        //     new Vector3(0, 1, 0),
        //     new Vector3(0, 1, 4),
        //     new Vector3(4, 1, 4),
        //     new Vector3(4, 0, 4),
        //     new Vector3(0, 0, 4),
        // };
        //
        //
        // triangleIndices = new List<int>
        // {
        //     0, 2, 1, //face front
        //     0, 3, 2,
        //     2, 3, 4, //face top
        //     2, 4, 5,
        //     1, 2, 5, //face right
        //     1, 5, 6,
        //     0, 7, 4, //face left
        //     0, 4, 3,
        //     5, 4, 7, //face back
        //     5, 7, 6,
        //     0, 6, 7, //face bottom
        //     0, 1, 6
        // };

        // vertices = new List<Vector3>
        // {
        //     new Vector3(0, 0, 0),
        //     new Vector3(4, 0, 0),
        //     new Vector3(4, 1, 0),
        //     new Vector3(0, 1, 0),
        //     new Vector3(0, 1, 4),
        //     new Vector3(4, 1, 4),
        //     new Vector3(4, 0, 4),
        //     new Vector3(0, 0, 4),
        // };
        //
        //
        // triangleIndices = new List<int>
        // {
        //     0, 2, 1, //face front
        //     0, 3, 2,
        //     2, 3, 4, //face top
        //     2, 4, 5,
        //     1, 2, 5, //face right
        //     1, 5, 6,
        //     0, 7, 4, //face left
        //     0, 4, 3,
        //     5, 4, 7, //face back
        //     5, 7, 6,
        //     0, 6, 7, //face bottom
        //     0, 1, 6
        // };

        triangles = new List<Triangle>();
        for (int i = 0; i < triangleIndices.Count; i += 3)
        {
            triangles.Add(new Triangle(
                transform.TransformPoint(vertices[triangleIndices[i]]), 
                transform.TransformPoint(vertices[triangleIndices[i + 1]]), 
                transform.TransformPoint(vertices[triangleIndices[i + 2]]),
                i / 3
            ));
        }
    }
    
    private void Update()
    {
        RenderBox();
    }
    
    private void RenderBox()
    {
        mesh.Clear();


        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangleIndices.ToArray();

        mesh.Optimize();
        mesh.RecalculateNormals();
    }

}
