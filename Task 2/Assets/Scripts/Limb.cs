using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Limb : MonoBehaviour
{
    public GameObject child;
    public GameObject control;
    public Vector3 jointLocation; // currently ??
    public Vector3 jointOffset; // initial ??
    public float angle;
    public float lastAngle;
    public Vector3[] limbVertexLocations;
    public Mesh mesh;

    void Awake()
    {
        DrawLimb();
    }

    void Start()
    {
        if (child != null) { child.GetComponent<Limb>().MoveByOffset(jointOffset); }
    }
    
    void Update()
    {
        lastAngle = angle;
        if (control != null) { angle = control.GetComponent<Slider>().value; } // use array of children, if childrenLength > 0...., for each child, angle = control.
        if (child != null) { child.GetComponent<Limb>().RotateAroundPoint( jointLocation, angle, lastAngle); }
        mesh.RecalculateBounds(); 
    }

    public void RotateAroundPoint(Vector3 point, float angle, float lastAngle)
    {
        // Move the point to the origin
        Matrix3x3 T1 = IGB283Transform.Translate(-point);
        // Undo the last rotation
        Matrix3x3 R1 = IGB283Transform.Rotate(-lastAngle);
        // Move the point back to the oritinal position
        Matrix3x3 T2 = IGB283Transform.Translate(point);
        // Perform the new rotation
        Matrix3x3 R2 = IGB283Transform.Rotate(angle);
        // The final translation matrix
        Matrix3x3 M = T2 * R2 * R1 * T1;

        // Move the mesh
        Vector3[] vertices = mesh.vertices;
        for(int i = 0; i < vertices.Length; i++) 
        {
            vertices[i] = M.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;
        // Apply the transformation to the joint
        jointLocation = M.MultiplyPoint(jointLocation);

        // Apply the transformation to the children
        if (child != null) 
        {
            child.GetComponent<Limb>().RotateAroundPoint( point, angle, lastAngle);
        }
    }

    private void DrawLimb()
    {
        // add meshfilter and meshrenderer
        // each limb should have a rectangle mesh with vertices at limbVertexLocations[0], [1], [2] and [3]
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        mesh.vertices = limbVertexLocations;

        // colors and triangles
        mesh.colors = new Color[] 
        {
            new Color(0.8f, 0.3f, 0.3f, 1.0f),
            new Color(0.8f, 0.3f, 0.3f, 1.0f),
            new Color(0.8f, 0.3f, 0.3f, 1.0f),
            new Color(0.8f, 0.3f, 0.3f, 1.0f)
        };
        mesh.triangles = new int[]{0, 1, 2, 0, 2, 3};
        mesh.RecalculateBounds();
    }

    public void MoveByOffset(Vector3 offset)
    {
        // find translation matrix
        Matrix3x3 T = IGB283Transform.Translate(offset);
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = T.MultiplyPoint(vertices[i]);
        }

        mesh.vertices = vertices;
        jointLocation = T.MultiplyPoint(jointLocation);

        if (child != null)
        {
            child.GetComponent<Limb>().MoveByOffset(offset);  
        }
    }
}