using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool isNodding;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isNodding)
        {
            Matrix3x3 N = IGB283Transform.Rotate(Mathf.Sin(Time.deltaTime));

        }
    }
    public static void Nodding()
    {

    }
}
