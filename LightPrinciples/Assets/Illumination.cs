// Calculating light vectors problem and caluclations here:
// https://docs.google.com/spreadsheets/d/1071TKfG_pqfm30-4Io3nsAkt5cnupYUbjQqdCeG7gV0/edit#gid=0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Illumination : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 poi = new Vector3(2.7f, -7.35f, 1.33f);
        Vector3 light = new Vector3(-4.83f, -2.69f, -2.36f);
        Vector3 camera = new Vector3(7.6f, 0.15f, -1.28f);
        Vector3 normal = new Vector3(0, -2.84f, 0);
        Vector3 reflection = new Vector3(4.83f, -2.69f, 2.36f);

        Debug.DrawLine(poi, poi + light, Color.white);
        Debug.DrawLine(poi, poi + camera, Color.red);
        Debug.DrawLine(poi, poi + normal, Color.yellow);
        Debug.DrawLine(poi, poi + reflection, Color.blue);
    }

    /*
       Any point can be reached on a sphere provided two references.

       Rotation around x: Inclination (i)
       Rotation around y: Azimuth     (a)

       X = Cx + r * sin(i) * sin(a)
       Y = Cy + r * cos(i)
       Z = Cz + r * sin(i) * cos(a)

       Remember to transform (i) and (a) into radians before proceeding!
    */
}
