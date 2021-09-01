using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTest : MonoBehaviour
{
    public Vector3 A; // Start of the line.
    public Vector3 B; // End of the line.

    public float t; // in A + t(B-A)

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        DrawAxes();
        DrawLine();
    }

    void DrawAxes()
    {
        Debug.DrawLine(Vector3.zero, new Vector3(10,0,0), Color.red);
        Debug.DrawLine(Vector3.zero, new Vector3(0,10,0), Color.green);
        Debug.DrawLine(Vector3.zero, new Vector3(0,0,10), Color.blue);
    }

    void DrawLine()
    {
        // y = mx + b, is only 2d and is not enough to represent a line.
        // 3D or any dimension:
        // A + t(B-A), where A, B are points in any dimension.
        // This uses the fact that A-B is a new vector.
        // 't' is a parameter that controls how much to move between A and B
        // A is the starting point B is the finish point of the line.
        // 't' is a percentage to be between A and the end B.
        
        // This definition gives a proper representation of a line
        // Called parametric equation of a line.
        // It will situate any point t percentage between A and B.

        // Draw the line
        Debug.DrawLine(A, B, Color.cyan);

        // Draw something moving in the line.
        Vector3 pos = A + t * (B -A);
        transform.position = pos;

        // Very important in Computer Graphics, 
        // Interpolation when t goes from 0 to 1 its called linear interpolation
        // Allows smooth transitions.
    }

}
