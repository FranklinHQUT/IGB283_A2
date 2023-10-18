using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoLimb : MonoBehaviour
{
    public GameObject[] children;
    public GameObject[] controls;
    public Vector3 jointLocation; // currently ??
    public Vector3 jointOffset; // initial ??
    public float[] angles;
    public float[] lastAngles;
    public Vector3[] limbVertexLocations;
    public Mesh mesh;
    public Material material;

    void Awake()
    {
        DrawLimb();
    }

    void Start()
    {
        if (children != null) 
        { 
            for (int i = 0; i < children.Length; i++)
            {
                children[i].GetComponent<TwoLimb>().MoveByOffset(jointOffset);
            }
        }
    }
    
    void Update()
    {
        //lastAngle = angle;
        // if (children.Length > 0)
        // {   
        //     if (control != null)
        //     {
        //         foreach (GameObject child in children) { angle = control.GetComponent<Slider>().value; }
        //     } 

        //     foreach (GameObject child in children) 
        //     {
        //         if (child != null)
        //         {
        //             child.GetComponent<TwoLimb>().RotateAroundPoint( jointLocation, angle, lastAngle ); 
        //         }
        //     }
        // }

        for (int i = 0; i < children.Length; i++)
        {
            lastAngles[i] = angles[i];
            if (controls[i] != null)
            {
                angles[i] = controls[i].GetComponent<Slider>().value;
                children[i].GetComponent<TwoLimb>().RotateAroundPoint( jointLocation, angles[i], lastAngles[i]);
            }
        }

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
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != null)
            {
                children[i].GetComponent<TwoLimb>().RotateAroundPoint( point, angles[i], lastAngles[i] );
            }
        }
    }

    private void DrawLimb()
    {
        // add meshfilter and meshrenderer
        // each limb should have a rectangle mesh with vertices at limbVertexLocations[0], [1], [2] and [3]
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        GetComponent<MeshRenderer>().material = material;
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

        if (children != null) 
        {
            foreach (GameObject child in children) { child.GetComponent<TwoLimb>().MoveByOffset(offset); }
        }
    }
}