using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalVector : MonoBehaviour
{
    public Vector3 A;
    public Vector3 B;
    public Vector3 C;

    // Start is called before the first frame update
    void Start()
    {
        DrawAxes();
    }

    // Update is called once per frame
    void Update()
    {
        DrawTriangle();
        DrawNormal();
    }

    void DrawAxes()
    {
        Debug.DrawLine(Vector3.zero, new Vector3(10,0,0), Color.red);
        Debug.DrawLine(Vector3.zero, new Vector3(0,10,0), Color.green);
        Debug.DrawLine(Vector3.zero, new Vector3(0,0,10), Color.blue);
    }

    void DrawTriangle()
    {
        Debug.DrawLine(A, B, Color.cyan);
        Debug.DrawLine(B, C, Color.cyan);
        Debug.DrawLine(C, A, Color.cyan);
    }

    void DrawNormal()
    {
        Vector3 AB = Mathematics.Subtract(A, B);
        Vector3 AC = Mathematics.Subtract(A, C);

        float rad = Mathematics.AngleBetween(A, B);
        float deg = Mathf.Rad2Deg * rad;
        Debug.Log("Angle between A and B: " + deg + "deg");

        // Right hand rule defines the direction of the normal.
        // Invert it, invert the cross product.
        Vector3 n = Mathematics.Cross(AB, AC);
        Debug.Log(Mathematics.Magnitude(n));

        Vector3 nu = Mathematics.Normalized(n);
        Debug.Log(nu); // Unitary "n"

        // Centroid average from the coordinates.
        Vector3 centroid = new Vector3((A.x+B.x+C.x)/3, (A.y + B.y+C.y)/3, (A.z+B.z+C.z)/3);
        Debug.DrawLine(centroid, centroid+n, Color.magenta);

        // When light hits the triangle it will bounce according to the normal.
    }
}
