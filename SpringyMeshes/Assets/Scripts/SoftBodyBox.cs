using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Random = System.Random;

public class SoftBodyBox : SoftBody
{

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
        //
        // particles[2].IsFixed = true;
        // particles[3].IsFixed = true;
        // particles[4].IsFixed = true;
        // particles[5].IsFixed = true;

        dampedSprings = new List<SBSDampedSpring>
        {
            // vertical
            new SBSDampedSpring(particles[1], particles[2], k, d),
            new SBSDampedSpring(particles[3], particles[0], k, d),
            new SBSDampedSpring(particles[5], particles[6], k, d),
            new SBSDampedSpring(particles[4], particles[7], k, d),

            // vertical diagonal
            new SBSDampedSpring(particles[0], particles[2], k, d),
            new SBSDampedSpring(particles[1], particles[3], k, d),
            new SBSDampedSpring(particles[1], particles[5], k, d),
            new SBSDampedSpring(particles[2], particles[6], k, d),
            new SBSDampedSpring(particles[4], particles[6], k, d),
            new SBSDampedSpring(particles[5], particles[7], k, d),
            new SBSDampedSpring(particles[0], particles[4], k, d),
            new SBSDampedSpring(particles[3], particles[7], k, d),


            // horizontal bottom
            new SBSDampedSpring(particles[0], particles[1], k, d),
            new SBSDampedSpring(particles[1], particles[6], k, d),
            new SBSDampedSpring(particles[6], particles[7], k, d),
            new SBSDampedSpring(particles[0], particles[7], k, d),

            // horizontal bottom diagonal
            new SBSDampedSpring(particles[0], particles[6], k, d),
            new SBSDampedSpring(particles[1], particles[7], k, d),

            // horizontal top
            new SBSDampedSpring(particles[2], particles[3], k, d),
            new SBSDampedSpring(particles[2], particles[5], k, d),
            new SBSDampedSpring(particles[3], particles[4], k, d),
            new SBSDampedSpring(particles[4], particles[5], k, d),

            // horizontal top diagonal
            new SBSDampedSpring(particles[2], particles[4], k, d),
            new SBSDampedSpring(particles[3], particles[5], k, d),


            new SBSDampedSpring(particles[0], particles[5], k, d),
            new SBSDampedSpring(particles[1], particles[4], k, d),
            new SBSDampedSpring(particles[3], particles[6], k, d),
            new SBSDampedSpring(particles[2], particles[7], k, d),
        };

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
}