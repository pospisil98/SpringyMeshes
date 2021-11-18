using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBody : MonoBehaviour
{
    public bool drawGizmos = false;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private List<SBNode> particles;
    private List<int> triangles;

    private void OnEnable()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        particles = new List<SBNode>
        {
            new SBNode(new Vector3(0, 0, 0), 1.0f),
            new SBNode(new Vector3(1, 0, 0), 1.0f),
            new SBNode(new Vector3(1, 1, 0), 1.0f),
            new SBNode(new Vector3(0, 1, 0), 1.0f),
            new SBNode(new Vector3(0, 1, 1), 1.0f),
            new SBNode(new Vector3(1, 1, 1), 1.0f),
            new SBNode(new Vector3(1, 0, 1), 1.0f),
            new SBNode(new Vector3(0, 0, 1), 1.0f),
        };

        particles[2].IsFixed = true;
        particles[3].IsFixed = true;
        particles[4].IsFixed = true;
        particles[5].IsFixed = true;

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
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].AddForce(new Vector3(0, -0.1f, 0));
        }

        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Tick(Time.deltaTime);
        }

        RenderCube();
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
            Gizmos.color = Color.red;
            if (particles != null)
            {
                for (int i = 0; i < particles.Count; i++)
                {
                    Gizmos.DrawSphere(transform.TransformPoint(particles[i].Position), 0.05f);
                }
            }
        }
    }
}
