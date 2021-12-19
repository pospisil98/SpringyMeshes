using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face
{
    public float angle1;
    public float angle2;
    public float angle3;

    public Strut s1;
    public Strut s2;
    public Strut s3;

    public Face(float angle1, float angle2, float angle3)
    {
        this.angle1 = angle1;
        this.angle2 = angle2;
        this.angle3 = angle3;
    }
}