using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpringyMeshMesh : SpringyMeshBase
{
    public GameObject model;

    protected override void InitMeshStuff()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = model.GetComponent<MeshFilter>().mesh;
        mesh = meshFilter.mesh;
    }
}