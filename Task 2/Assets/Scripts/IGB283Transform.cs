using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGB283Transform
{
    // put rotate method in here, make static so I can do .Rotate()
    public static Matrix3x3 Rotate (float angle) 
	{
		Matrix3x3 matrix = new Matrix3x3();
		// set the matrix's rows
		matrix.SetRow(0, new Vector3(Mathf.Cos(angle), -Mathf.Sin(angle), 0.0f));
		matrix.SetRow(1, new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0.0f));
		matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));

		return matrix;
	}

    public static Matrix3x3 Translate(Vector3 offset)
    {
        Matrix3x3 matrix = new Matrix3x3();

        matrix.SetRow(0, new Vector3(1.0f, 0.0f, offset.x));
        matrix.SetRow(1, new Vector3(0.0f, 1.0f, offset.y));
        matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));

        return matrix;
    }

	public static Matrix3x3 Scale(float scale)
	{
		Matrix3x3 matrix = new Matrix3x3();

		//scaledMatrix = [scalex 0, 0 scaley]
		matrix.SetRow(0, new Vector3(scale, 0.0f, 0.0f));
		matrix.SetRow(1, new Vector3(0.0f, scale, 0.0f));
		matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));

		return matrix;
	}

}
