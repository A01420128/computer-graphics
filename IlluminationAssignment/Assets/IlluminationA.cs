using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Computer grafics
// October 1, 2021
// ILLUMINATION Assignment

// Enrique Orduna - A01027318 is A
// Javier Flores - A01651678 is B
// Jose Javier Tlacuilo - A01420128 is C

public class IlluminationA : MonoBehaviour
{
    IlluminationData enrique;
    IlluminationData javier;
    IlluminationData tlacuilo;
    // Start is called before the first frame update
    void Start()
    {
        enrique = new IlluminationData("enrique");
        javier = new IlluminationData("javier");
        tlacuilo = new IlluminationData("tlacuilo");

        DisplaySphere(enrique);
        DisplaySphere(javier);
        DisplaySphere(tlacuilo);
        enrique.PrintData();
        javier.PrintData();
        tlacuilo.PrintData();
    }

    // Update is called once per frame
    void Update()
    {
        DisplayVectors(enrique);
        DisplayVectors(javier);
        DisplayVectors(tlacuilo);
    }

    void DisplaySphere(IlluminationData data) {
        Matrix4x4 tm = Transformations.TranslateM(data.TA.x, data.TA.y, data.TA.z);
        Vector4 A2 = new Vector4(data.A.x, data.A.y, data.A.z, 1);
        Vector4 A3 = tm * A2;

        Matrix4x4 rm;
        if (data.axis == "X")
        {
            rm = Transformations.RotateM(data.rotation, Transformations.AXIS.AX_X);
        } else if (data.axis == "Y")
        {
            rm = Transformations.RotateM(data.rotation, Transformations.AXIS.AX_Y);
        } else if (data.axis == "Z")
        {
            rm = Transformations.RotateM(data.rotation, Transformations.AXIS.AX_Z);
        } else {
            rm = Matrix4x4.zero;
        }

        // Take to the origin.
        Matrix4x4 tm2 = Transformations.TranslateM(-data.P.x, -data.P.y, -data.P.z);
        Vector4 A4 = tm2 * A3;
        // Rotate
        Vector4 A5 = rm * A4;
        // Take back:
        Matrix4x4 tm3 = Transformations.TranslateM(data.P.x, data.P.y, data.P.z);
        data.CENTER = tm3 * A5;

        Vector3 cartesian = Mathematics.SphericalToCartesian(data.i, data.a, data.sphradius);
        data.PoI = data.CENTER + cartesian;


        // ILLUMINATION MATH
        data.n = data.PoI - data.CENTER;
        Vector3 nu = Mathematics.Normalized(data.n);

        data.l = data.LIGHT - data.PoI;
        Vector3 lu = Mathematics.Normalized(data.l);

        data.v = data.CAMERA - data.PoI;
        Vector3 vu = Mathematics.Normalized(data.v);

        data.r = Mathematics.Reflect(data.l, data.n);
        Vector3 ru = Mathematics.Normalized(data.r);

        float dotnulu = Mathematics.Dot(nu, lu);
        float dotvuru = Mathematics.Dot(vu, ru);
        float dvra = Mathf.Pow(dotvuru, data.alpha);

        data.ar = data.ka.x * data.Ia.x;
        data.ag = data.ka.y * data.Ia.y;
        data.ab = data.ka.z * data.Ia.z;

        data.dr = data.kd.x * data.Id.x * dotnulu;
        data.dg = data.kd.y * data.Id.y * dotnulu;
        data.db = data.kd.z * data.Id.z * dotnulu;

        data.sr = data.ks.x * data.Is.x * dvra;
        data.sg = data.ks.y * data.Is.y * dvra;
        data.sb = data.ks.z * data.Is.z * dvra;

        data.finalColor = new Vector3(data.ar+data.dr+data.sr, data.ag+data.dg+data.sg, data.ab+data.db+data.sb);

        int hR = (int)(data.finalColor.x * 255.0f);
        int hG = (int)(data.finalColor.y * 255.0f);
        int hB = (int)(data.finalColor.z * 255.0f);

        string hexR = hR.ToString("X2");
        string hexG = hG.ToString("X2");
        string hexB = hB.ToString("X2");
        data.hexColor = "#" + hexR + hexG + hexB;
        // Expected: 0.35346 0.39961 0.74096

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = data.CENTER;
        sphere.transform.localScale = new Vector3(data.sphradius*2, data.sphradius*2, data.sphradius*2);
        sphere.name = data.sphereName;

        // Changing the properties of the material.
        Renderer sphR = sphere.GetComponent<Renderer>();
        sphR.material = new Material(Shader.Find("Specular"));
        //Setting color
        Color diffuse = new Color(data.dr, data.dg, data.db);
        Color specular = new Color(data.sr, data.sg, data.sb);
        sphR.material.SetColor("_Color", diffuse);
        sphR.material.SetColor("_SpecColor", specular);

        // Changing the properties of the Camera.
        GameObject camera = GameObject.Find(data.cameraName);
        camera.transform.position = data.CAMERA;
        camera.transform.LookAt(sphere.transform);
        camera.GetComponent<AudioListener>().enabled  =  false;

        // Changing the properties of the light.
        Light light = GameObject.Find(data.lightName).GetComponent<Light>();
        light.transform.position = data.LIGHT;
        light.type = LightType.Point;
        light.color = new Color(data.Id.x, data.Id.y, data.Id.z);
        light.intensity = 5;
    }

    void DisplayVectors(IlluminationData data) {
        Debug.DrawLine(data.PoI, data.PoI + data.n, Color.blue);
        Debug.DrawLine(data.PoI, data.PoI + data.l, Color.red);
        Debug.DrawLine(data.PoI, data.PoI + data.v, Color.green);
        Debug.DrawLine(data.PoI, data.PoI + data.r, Color.white);
    }
}

public class IlluminationData {

    public Vector3 ka;
    public Vector3 kd;
    public Vector3 ks;
    
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
    public Vector3 CENTER;

    public Vector3 n;
    public Vector3 l;
    public Vector3 v;
    public Vector3 r;

    public float ar;
    public float ag;
    public float ab;

    public float dr;
    public float dg;
    public float db;

    public float sr;
    public float sg;
    public float sb;

    public Vector3 finalColor;
    public string hexColor;

    public string cameraName;
    public string lightName;
    public string sphereName;

    public string dataName;

    public IlluminationData(string who) {
        switch (who) {
            case "enrique":
                ka = new Vector3(0.22706f, 0.27792f, 0.20751f);
                kd = new Vector3(0.86762f, 0.17715f, 0.08411f);
                ks = new Vector3(0.11494f, 0.10535f, 0.49644f);

                Ia = new Vector3(0.92661f, 0.98887f, 0.93017f);
                Id = new Vector3(0.95803f, 0.87745f, 0.87835f);
                Is = new Vector3(0.83910f, 0.98693f, 0.84901f);

                alpha=37;
                sphradius=0.56922f;

                A = new Vector3(-1.82023f, 1.42825f, 1.09555f);
                TA = new Vector3(-0.78250f, 0.96390f, 0.46837f);
                P = new Vector3(-0.70569f, 0.69819f, 0.45843f);
                axis="Y";
                rotation=2.9f;
                i=33;
                a=282;

                LIGHT = new Vector3(-3.26116f, 3.22144f, 4.44079f);
                CAMERA = new Vector3(-4.53647f, 4.45523f, -6.24143f);

                dataName = "enriqueA";
                cameraName = "CameraA";
                lightName = "LightA";
                sphereName = "SphereA";
                break;
            case "javier":
                 // Material Properties: (R,G,B)
                ka = new Vector3(0.19662f, 0.21149f, 0.23563f );
                kd = new Vector3(0.60373f, 0.08085f, 0.10724f );
                ks = new Vector3(0.22949f, 0.24814f, 0.45767f );

                // Light Properties: (R,G,B)
                Ia = new Vector3(0.89778f, 0.80402f, 0.97804f );
                Id = new Vector3(0.87344f, 0.96167f, 0.81818f);
                Is = new Vector3(0.94525f, 0.84443f, 0.94775f );

                alpha = 168;
                sphradius = 0.92292f;

                A = new Vector3(1.28421f, 1.29329f, 0.09507f);
                TA = new Vector3(0.85175f, 0.85044f, 0.10648f);
                P = new Vector3(0.30838f,  0.60816f, 0.61045f);
                axis = "X";
                rotation = -2.2f;
                i = 40;
                a = 79;

                LIGHT = new Vector3(3.64196f, 4.14005f, 4.24436f);
                CAMERA = new Vector3(5.26184f, 4.91518f, -5.85887f);

                dataName = "javierB";
                cameraName = "CameraB";
                lightName = "LightB";
                sphereName = "SphereB";
                break;
            case "tlacuilo":
                ka = new Vector3(0.12751f, 0.15715f, 0.22332f);
                kd = new Vector3(0.17987f, 0.16062f, 0.61673f);
                ks = new Vector3(0.19430f, 0.39928f, 0.39389f);

                Ia = new Vector3(0.89358f, 0.86110f, 0.80275f);
                Id = new Vector3(0.82205f, 0.80737f, 0.87130f);
                Is = new Vector3(0.98890f, 0.95062f, 0.98042f);

                alpha = 58;
                sphradius = 0.58728f;

                A = new Vector3(0.59885f, -0.71711f, -0.02898f);
                TA = new Vector3(0.49417f, -0.18950f, -0.21688f);
                P = new Vector3(.09273f, -0.30277f, -0.03936f);
                axis = "X";
                rotation = -3.7f;
                i = 121;
                a = 104;

                LIGHT = new Vector3(4.70160f, -3.61099f, -3.12989f);
                CAMERA = new Vector3(5.60330f, -3.68435f, 1.31879f);

                dataName = "tlacuiloC";
                cameraName = "CameraC";
                lightName = "LightC";
                sphereName = "SphereC";
                break;
        }
    }

    public void PrintData() {
        Debug.Log(dataName + " >> PoI: " + PoI.ToString("F5"));
        Debug.Log(dataName + " >> Light vector: " + PoI.ToString("F5"));
        Debug.Log(dataName + " >> Normal vector" + PoI.ToString("F5"));
        Debug.Log(dataName + " >> Reflected vector" + PoI.ToString("F5"));
        Debug.Log(dataName + " >> Ar " + ar.ToString("F5") + ", Ag " + ag.ToString("F5") + ", Ab " + ab.ToString("F5"));
        Debug.Log(dataName + " >> Dr " + dr.ToString("F5") + ", Dg " + dg.ToString("F5") + ", Db " + db.ToString("F5"));
        Debug.Log(dataName + " >> Sr " + sr.ToString("F5") + ", Sg " + sg.ToString("F5") + ", Sb " + sb.ToString("F5"));
        Debug.Log(dataName + " >> Final color: " + finalColor.ToString("F5"));
        Debug.Log(dataName + " >> HexColor: " + hexColor);
    }
}
