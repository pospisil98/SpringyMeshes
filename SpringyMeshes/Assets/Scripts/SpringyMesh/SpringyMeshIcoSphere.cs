using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpringyMeshIcoSphere : SpringyMeshBase
{
    public int subdivisions;
    public float radius;

    protected override void InitMeshStuff()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = ShapeGenerator.IcoSphere.Create(subdivisions, radius);

        mesh = meshFilter.mesh;

    }

    protected override void InitObject()
    {
        base.InitObject();
        // vertices[0].isFixed = true;
    }

    protected override void Update()
    {
        Render();
        ProcessInput();
    }
}