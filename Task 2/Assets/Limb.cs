using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Limb : MonoBehaviour
{ 
    public GameObject child;
    public GameObject control;
    public GameObject parent;
    public Vector3 jointLocation;
    public Vector3 jointOffset;
    public float angle;
    public float lastAngle;
    public Vector3[] limbVertexLocations;
    public Mesh mesh;
    public Material material;
    public int[] Vertexorder;
    public bool isNodding;
    public float nodAngle;
    public int HeadingPosNum;
    public float childHeading;
    public Vector3 offset;
    public Vector3 pointBeingCompared;
    public bool jumpPressed;
    public bool wasLastDirectionRight;
    public GameObject prefabForCollision;
    public int NumOfCollisions;

    // This will run before Start
    void Awake()
    {
        // Draw the limb
        DrawLimb();
    }

    void Start () 
    {
        // Move the child to the joint location
        if (child != null) {
            child.GetComponent<Limb>().MoveByOffset(jointOffset);
        }
        offset.x = 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        //get heading and last angle values
        lastAngle = angle;
        if (parent != null)
        {
            pointBeingCompared = mesh.vertices[HeadingPosNum];
            childHeading = angleFromXaxis(parent.GetComponent<Limb>().jointLocation, pointBeingCompared);
        }

        //position secitions of the limb and rotate them around joint point if angle != null

        if (control != null)
        {
            angle = control.GetComponent<Slider>().value;
        }
        if (child != null)
        {
            child.GetComponent<Limb>().RotateAroundPoint(jointLocation, angle, lastAngle);
            childHeading = child.GetComponent<Limb>().childHeading;
        }

        // nodding
        if (isNodding) { Nod(); }

        //movement left and right
        if (Input.GetKey(KeyCode.D)) { offset.x = 0.05f; }
        if (Input.GetKey(KeyCode.A)) { offset.x = -0.05f; }
        if (Input.GetKey(KeyCode.S)) { offset.x = 0.0f; }
        if (parent == null)
        {
            if (jointLocation.x > 20)
            {
                offset.x = -offset.x;
                ChangeDirectionSpawn();
                wasLastDirectionRight = false;
            }
            if (jointLocation.x < - 20)
            {
                offset.x = -offset.x;
                wasLastDirectionRight = true;
            }
            MoveByOffset(offset);
        }

        if (offset.x > 0) { wasLastDirectionRight = true; } else { wasLastDirectionRight = false; }

        Jump();      

        // Recalculate the bounds of the mesh
        mesh.RecalculateBounds();
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.W)) // Check if "W" is pressed and it's the top parent
        {   
            if (parent == null)
            {
                if (jointLocation.y <= -2) // Check if the limb is on the floor
                    {
                        offset.y = 0.05f; // If on the floor, go up
                    }
            }
        }
        if ((offset.x == 0) && (Input.GetKey(KeyCode.S)))
        {
            if (parent == null)
            {
                if (jointLocation.y <= -2)
                {
                    offset.y = 0.05f;
                    ChangeDirectionSpawn();
                    if (wasLastDirectionRight)
                    {
                        offset.x = 0.05f;
                    }
                    else
                    {
                        offset.x = -0.05f;
                    }
                }
            }
        }
        else if (offset.y > 0) // Check if the limb is going up
        {
            if (jointLocation.y > 4) { offset.y = 0.04f; }
            if (jointLocation.y >= 6) // If it's at the top, start going down
            {
                ChangeDirectionSpawn();
                offset.y = -0.05f;
            }
        }
        else if (offset.y < 0) // Check if the limb is going down
        {
            if (jointLocation.y <= -2) // If it's at the bottom, stop going down
            {
                ChangeDirectionSpawn();
                offset.y = 0f;
                offset.x = 0f;
            }
        }

        if (parent == null)
        {
            MoveByOffset(offset);
        }
    }

    public void ChangeDirectionSpawn()
    {
        if (NumOfCollisions > 0)
        {
            Instantiate(prefabForCollision, new Vector3(jointLocation.x, jointLocation.y, jointLocation.z), Quaternion.identity);
            NumOfCollisions -= 1;
        }
    }

    private void DrawLimb()
    {
        ///Create the mesh--------------------------------------------------------------
        // Add a MeshFilter and MeshRenderer to the Empty GameObject
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        // Get the Mesh from the MeshFilter
        mesh = GetComponent<MeshFilter>().mesh;
        // Set the material to the material we have selected
        GetComponent<MeshRenderer>().material = material;
        // Clear all vertex and index data from the mesh
        mesh.Clear();

        //fill mesh with verticies
        mesh.vertices = limbVertexLocations;

        // Set the colour of the shape
        Color[] colors = new Color[limbVertexLocations.Length];
        for(int i=0; i<colors.Length; i++)
        {
            Color color = new Color(0.8f, 0.3f, 0.3f, 1.0f);
            colors[i] = color;
        }
        mesh.colors = colors;

        // Set vertex indicies
        mesh.triangles = Vertexorder;
    }

    public void MoveByOffset(Vector3 offset)
    {
        // Find the translation Matrix
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


    // Rotate the limb around a point
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
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = M.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;
        // Apply the transformation to the joint
        jointLocation = M.MultiplyPoint(jointLocation);

        // Apply the transformation to the children
        if (child != null)
        {
            child.GetComponent<Limb>().RotateAroundPoint(point, angle, lastAngle);
        }
    }

    public static float angleFromXaxis(Vector3 rotationPoint, Vector3 point)
    {
        Vector3 direction = point - rotationPoint;
        float angleFromX = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angleFromX < 0)
        {
            angleFromX += 360;
        }
        return angleFromX;
    }

    void Scaling(Matrix3x3 scalingMat)
    {
        // Apply the scaling matrix to the limb's vertices
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = scalingMat.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;

        // Apply the scaling matrix to the joint location
        jointLocation = scalingMat.MultiplyPoint(jointLocation);

        // Apply the scaling matrix to the children
        if (child != null)
        {
            child.GetComponent<Limb>().Scaling(scalingMat);
        }
    }

    void Nod()
    {
        // Define the nodding angle range
        float minNodAngle = 60.0f;
        float maxNodAngle = 120.0f;

        if (childHeading < minNodAngle)
        {
            nodAngle = Mathf.Abs(nodAngle); // Ensure a positive nod angle
        }
        else if (childHeading > maxNodAngle)
        {
            nodAngle = -Mathf.Abs(nodAngle); // Ensure a negative nod angle
        }

        child.GetComponent<Limb>().RotateAroundPoint(jointLocation, nodAngle, lastAngle);
    }

}