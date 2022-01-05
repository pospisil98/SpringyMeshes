using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SpringyMeshPrimitive : SpringyMeshBase
{
    public int size = 4;
    public ShapeGenerator.ShapeType shapeType;
    
    protected override void InitMeshStuff()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        meshFilter = gameObject.AddComponent<MeshFilter>();

        if (shapeType == ShapeGenerator.ShapeType.Cube) {
            meshFilter.mesh = ShapeGenerator.Cube.Create(size);
        } else if (shapeType == ShapeGenerator.ShapeType.Tetrahedron) {
            meshFilter.mesh = ShapeGenerator.Tetrahedron.Create();
        }

        mesh = meshFilter.mesh;
    }
}