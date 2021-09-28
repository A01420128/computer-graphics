using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
   ILLUMINATION EXCERSICE FROM SLACK
   https://files.slack.com/files-pri/T02ATULBCBW-F02FZ802Z3M/20210928_151922.png
*/

public class ExerCube : MonoBehaviour
{
    // Material Properties: (R,G,B)
    public Vector3 ka;
    public Vector3 kd;
    public Vector3 ks;
    
    // Light Properties: (R,G,B)
    public Vector3 Ia;
    public Vector3 Id;
    public Vector3 Is;

    public float alpha;
    public float side;

    public Vector3 CENTER;
    public Vector3 PoI;
    public Vector3 LIGHT;
    public Vector3 CAMERA;

    public Vector3 n;
    public Vector3 l;
    public Vector3 v;
    public Vector3 r;

    // Start is called before the first frame update
    void Start()
    {
        // ILLUMINATION MATH
        n = PoI - CENTER;
        Vector3 nu = Mathematics.Normalized(n);

        l = LIGHT - PoI;
        Vector3 lu = Mathematics.Normalized(l);

        v = CAMERA - PoI;
        Vector3 vu = Mathematics.Normalized(v);

        r = Mathematics.Reflect(l, n);
        Vector3 ru = Mathematics.Normalized(r);

        float dotnulu = Mathematics.Dot(nu, lu);
        float dotvuru = Mathematics.Dot(vu, ru);
        float dvra = Mathf.Pow(dotvuru, alpha);

        float ar = ka.x * Ia.x;
        float ag = ka.y * Ia.y;
        float ab = ka.z * Ia.z;

        float dr = kd.x * Id.x * dotnulu;
        float dg = kd.y * Id.y * dotnulu;
        float db = kd.z * Id.z * dotnulu;

        float sr = ks.x * Is.x * dvra;
        float sg = ks.y * Is.y * dvra;
        float sb = ks.z * Is.z * dvra;

        Vector3 color = new Vector3(ar+dr+sr, ag+dg+sg, ab+db+sb);
        Debug.Log("COLOR: " + color.ToString("F5"));
        Debug.Log("SPEC: " + sr + ", " + sg + ", " + sb);

        int hR = (int)(color.x * 255.0f);
        int hG = (int)(color.y * 255.0f);
        int hB = (int)(color.z * 255.0f);

        string hexR = hR.ToString("X2");
        string hexG = hG.ToString("X2");
        string hexB = hB.ToString("X2");
        Debug.Log("#" + hexR + hexG + hexB);
        // Expected: 0.35346 0.39961 0.74096

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = CENTER;
        cube.transform.localScale = new Vector3(side, side, side);

        // Changing the properties of the material.
        Renderer cubeR = cube.GetComponent<Renderer>();
        cubeR.material = new Material(Shader.Find("Specular"));
        //Setting color
        Color diffuse = new Color(dr, dg, db);
        Color specular = new Color(sr, sg, sb);
        cubeR.material.SetColor("_Color", diffuse);
        cubeR.material.SetColor("_SpecColor", specular);

        // Changing the properties of the Camera.
        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.position = CAMERA;
        camera.transform.LookAt(cube.transform);

        // Changing the properties of the light.
        Light light = GameObject.Find("Directional Light").GetComponent<Light>();
        light.transform.position = LIGHT;
        light.type = LightType.Point;
        light.color = new Color(Id.x, Id.y, Id.z);
        light.intensity = 5;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(PoI, PoI + n, Color.red);
        Debug.DrawLine(PoI, PoI + l, Color.green);
        Debug.DrawLine(PoI, PoI + v, Color.blue);
        Debug.DrawLine(PoI, PoI + r, Color.cyan);
    }
}
