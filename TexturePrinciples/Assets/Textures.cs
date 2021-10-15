using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
   ILLUMINATION DATA ADDS PROPERTIES
    kar = 0.13793 kag = 0.21465 kab = 0.24848
    kdr = 0.14137 kdg = 0.20173 kdb = 0.70740
    ksr = 0.35248 ksg = 0.21094 ksb = 0.45698
    Iar = 0.82922 Iag = 0.94467 Iab = 0.85464
    Idr = 0.97740 Idg = 0.85867 Idb = 0.80462
    Isr = 0.81855 Isg = 0.89442 Isb = 0.92152
    alpha = 49
    GEOMETRY DATA
    Sphere radius:           SR =  0.82271
    To obtain the Sphere Center (SC), do the following HOMOGENEOUS TRANSFORMATIONS:
    Consider point A = (-1.88826, -1.00465, -0.42868)
        1. A' is: translate point A by (-0.59509, -0.04527, -0.39456)
        2. Consider the pivot P = (-0.20854, -0.87605, -0.45395)
        3. SC is: rotate point A' by -1.6° around P with axis Z.
    PoI inclination angle:    i = 165°
    PoI azimuth angle:        a = 246°
    SCENE DATA
    Point light position: LIGHT = (-3.09002, -3.28094, -4.53538)
    Camera position:        CAM = (-4.07321, -4.64437,  4.37821)
*/

public class Textures : MonoBehaviour
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
    public float sphradius;

    public Vector3 A;
    public Vector3 TA;
    public Vector3 P;
    public string axis;
    public float rotation;
    public float i;
    public float a;

    public Vector3 LIGHT;
    public Vector3 CAMERA;

    public Vector3 PoI;

    public Vector3 n;
    public Vector3 l;
    public Vector3 v;
    public Vector3 r;

    public Texture aTexture;

    // Start is called before the first frame update
    void Start()
    {
        Matrix4x4 tm = Transformations.TranslateM(TA.x, TA.y, TA.z);
        Vector4 A2 = new Vector4(A.x, A.y, A.z, 1);
        Vector4 A3 = tm * A2;

        Matrix4x4 rm;
        if (axis == "X")
        {
            rm = Transformations.RotateM(rotation, Transformations.AXIS.AX_X);
        } else if (axis == "Y")
        {
            rm = Transformations.RotateM(rotation, Transformations.AXIS.AX_Y);
        } else if (axis == "Z")
        {
            rm = Transformations.RotateM(rotation, Transformations.AXIS.AX_Z);
        } else {
            rm = Matrix4x4.zero;
        }

        // Take to the origin.
        Matrix4x4 tm2 = Transformations.TranslateM(-P.x, -P.y, -P.z);
        Vector4 A4 = tm2 * A3;
        // Rotate
        Vector4 A5 = rm * A4;
        // Take back:
        Matrix4x4 tm3 = Transformations.TranslateM(P.x, P.y, P.z);
        Vector3 CENTER = tm3 * A5;
        Debug.Log("CENTER: " + CENTER.ToString("F5"));


        Vector3 cartesian = Mathematics.SphericalToCartesian(i, a, sphradius);
        PoI = CENTER + cartesian;
        Debug.Log("PoI: " + PoI.ToString("F5"));


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

        int hR = (int)(color.x * 255.0f);
        int hG = (int)(color.y * 255.0f);
        int hB = (int)(color.z * 255.0f);

        string hexR = hR.ToString("X2");
        string hexG = hG.ToString("X2");
        string hexB = hB.ToString("X2");
        Debug.Log("#" + hexR + hexG + hexB);
        // Expected: 0.35346 0.39961 0.74096

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = CENTER;
        sphere.transform.localScale = new Vector3(sphradius*2, sphradius*2, sphradius*2);

        // Changing the properties of the material.
        Renderer sphR = sphere.GetComponent<Renderer>();
        sphR.material = new Material(Shader.Find("Specular"));
        //Setting color
        Color diffuse = new Color(dr, dg, db);
        Color specular = new Color(sr, sg, sb);
        sphR.material.SetColor("_Color", diffuse);
        sphR.material.SetColor("_SpecColor", specular);

        // Apply the texture to the material.
        sphR.material.SetTexture("_MainTex", aTexture);

        // Changing the properties of the Camera.
        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.position = CAMERA;
        camera.transform.LookAt(sphere.transform);

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
        Debug.DrawLine(PoI, PoI + l, Color.yellow);
        Debug.DrawLine(PoI, PoI + v, Color.blue);
        Debug.DrawLine(PoI, PoI + r, Color.cyan);
    }
}
