using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Computer grafics
// October 13, 2021
// Texture Assignment

// Enrique Orduna - A01027318 is A
// Javier Flores - A01651678 is B
// Jose Javier Tlacuilo - A01420128 is C

public class IlluminationA : MonoBehaviour
{
    IlluminationData enrique;
    IlluminationData javier;
    IlluminationData tlacuilo;

    public Texture[] aTexture = new Texture[3];

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

        Vector3 cartesian = Mathematics.SphericalToCartesian(data.inc, data.azi, data.sphradius);
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

        //Texture
        int TEXTURE_SIDE = 5;
        data.uv = Mathematics.SphericalMapping(nu);
        int s = (int)(data.uv.x * TEXTURE_SIDE);
        int t = TEXTURE_SIDE - (int)(data.uv.y * TEXTURE_SIDE) - 1;
        Vector3 textRGB = data.texturaArray[t, s];
        data.texturePatch = data.texturasLetras[t, s];


        // Final color with textures
        data.finalColor = new Vector3((data.ar+data.dr+data.sr)* textRGB.x, (data.ag+data.dg+data.sg) * textRGB.y, (data.ab+data.db+data.sb) * textRGB.z);

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
        sphR.material.SetTexture("_MainTex", aTexture[data.textureIdx]); // Defined as public Texture aTexture and dragged in unity.

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

    public Vector3 Ap;
    public Vector3 TA;
    public Vector3 Piv;
    public string axis;
    public float rotation;
    public float inc;
    public float azi;
    public Vector2 uv;

    public Vector3 LIGHT;
    public Vector3 CAMERA;
    public Vector3 PoI;
    public Vector3 CENTER;

    public Vector3 n;
    public Vector3 l;
    public Vector3 v;
    public Vector3 r;

    public int textureIdx;

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

    public Vector3 A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y;
    public Vector3[,] texturaArray;
    public string texturePatch;
    public string[,] texturasLetras = new string[,] {
        {"A", "B", "C", "D", "E"},
        {"F", "G", "H", "I", "J"},
        {"K", "L", "M", "N", "O"},
        {"P", "Q", "R", "S", "T"},
        {"U", "V", "W", "X", "Y"}
    };

    public IlluminationData(string who) {
        switch (who) {
            case "enrique":
                ka = new Vector3(0.12151f, 0.29383f, 0.29974f);
                kd = new Vector3(0.91772f, 0.08127f, 0.06302f);
                ks = new Vector3(0.48669f, 0.34077f, 0.34528f);

                Ia = new Vector3(0.91695f, 0.92610f, 0.89676f);
                Id = new Vector3(0.86362f, 0.86361f, 0.95644f);
                Is = new Vector3(0.89003f, 0.92461f, 0.97425f);

                alpha= 52;
                sphradius= 0.90677f;

                Ap = new Vector3(1.80143f, 1.59398f, 0.39506f);
                TA = new Vector3(0.76131f, 0.56086f, 0.36885f);
                Piv = new Vector3(0.71706f, 0.97475f, 0.97860f);
                axis="X";
                rotation=-0.2f;
                inc=22;
                azi=72;

                LIGHT = new Vector3(3.11451f, 3.09378f, 4.98327f);
                CAMERA = new Vector3(4.09549f, 4.08832f, -5.53693f);

                dataName = "enriqueA";
                cameraName = "CameraA";
                lightName = "LightA";
                sphereName = "SphereA";

                A = new Vector3(0.74910f, 0.68228f, 0.71185f);
                B = new Vector3(0.65030f, 0.91470f, 0.89696f);
                C = new Vector3(0.84923f, 0.61558f, 0.63845f);
                D = new Vector3(0.73459f, 0.61800f, 0.84007f);
                E = new Vector3(0.65193f, 0.74907f, 0.87210f);
                F = new Vector3(0.76835f, 0.74809f, 0.88385f);
                G = new Vector3(0.83587f, 0.74999f, 0.86483f);
                H = new Vector3(0.72021f, 0.77096f, 0.87646f);
                I = new Vector3(0.71307f, 0.95816f, 0.67190f);
                J = new Vector3(0.66197f, 0.81671f, 0.83921f);
                K = new Vector3(0.91167f, 0.99743f, 0.82292f);
                L = new Vector3(0.88856f, 0.85974f, 0.92339f);
                M = new Vector3(0.96130f, 0.85925f, 0.83831f);
                N = new Vector3(0.60512f, 0.62677f, 0.88441f);
                O = new Vector3(0.96780f, 0.92330f, 0.70916f);
                P = new Vector3(0.97348f, 0.95835f, 0.84467f);
                Q = new Vector3(0.70185f, 0.81831f, 0.79366f);
                R = new Vector3(0.89496f, 0.63093f, 0.90599f);
                S = new Vector3(0.66730f, 0.96066f, 0.82643f);
                T = new Vector3(0.77402f, 0.79557f, 0.99521f);
                U = new Vector3(0.70996f, 0.90155f, 0.64827f);
                V = new Vector3(0.75197f, 0.63204f, 0.92928f);
                W = new Vector3(0.69732f, 0.92082f, 0.72152f);
                X = new Vector3(0.96322f, 0.84937f, 0.97313f);
                Y = new Vector3(0.60502f, 0.87742f, 0.95177f);

                texturaArray = new Vector3[,] { { A, B, C, D, E },
                                                { F, G, H, I, J },
                                                { K, L, M, N, O },
                                                { P, Q, R, S, T },
                                                { U, V, W, X, Y }};
                textureIdx = 0;
                break;
            case "javier":
                 // Material Properties: (R,G,B)
                ka = new Vector3(0.16434f, 0.24935f, 0.20689f);
                kd = new Vector3(0.77253f, 0.04800f, 0.03879f);
                ks = new Vector3(0.17501f, 0.37001f, 0.18798f);

                // Light Properties: (R,G,B)
                Ia = new Vector3(0.87761f, 0.98008f, 0.96141f);
                Id = new Vector3(0.99391f, 0.89605f, 0.98083f);
                Is = new Vector3(0.81242f, 0.96888f, 0.89453f);

                alpha = 24;
                sphradius = 0.57531f;

                Ap = new Vector3(-1.06132f, 0.00898f, -1.10362f);
                TA = new Vector3(-0.81508f, 0.84721f, -0.12319f);
                Piv = new Vector3(-0.38705f, 0.34036f, -0.89211f);
                axis = "Z";
                rotation = 9.2f;
                inc = 29;
                azi = 256;

                LIGHT = new Vector3(-3.54642f, 3.29080f, -3.60046f);
                CAMERA = new Vector3(-4.96556f, 4.59073f, 1.98812f);

                dataName = "javierB";
                cameraName = "CameraB";
                lightName = "LightB";
                sphereName = "SphereB";

                A = new Vector3(0.68067f, 0.83783f, 0.96851f);
                B = new Vector3(0.87140f, 0.65587f, 0.77584f);
                C = new Vector3(0.72753f, 0.91304f, 0.68758f);
                D = new Vector3(0.95072f, 0.79281f, 0.67525f);
                E = new Vector3(0.79029f, 0.95457f, 0.76127f);
                F = new Vector3(0.61957f, 0.91425f, 0.99065f);
                G = new Vector3(0.69972f, 0.70921f, 0.97008f);
                H = new Vector3(0.74292f, 0.83317f, 0.94407f);
                I = new Vector3(0.93780f, 0.67934f, 0.85960f);
                J = new Vector3(0.97111f, 0.83719f, 0.67807f);
                K = new Vector3(0.82185f, 0.84109f, 0.91245f);
                L = new Vector3(0.96078f, 0.90143f, 0.97719f);
                M = new Vector3(0.60089f, 0.75521f, 0.64202f);
                N = new Vector3(0.77522f, 0.91966f, 0.98562f);
                O = new Vector3(0.62363f, 0.85198f, 0.65702f);
                P = new Vector3(0.90413f, 0.83457f, 0.74064f);
                Q = new Vector3(0.96489f, 0.73564f, 0.65089f);
                R = new Vector3(0.89546f, 0.62053f, 0.92620f);
                S = new Vector3(0.83747f, 0.96743f, 0.68920f);
                T = new Vector3(0.97906f, 0.91743f, 0.75670f);
                U = new Vector3(0.74214f, 0.85941f, 0.68447f);
                V = new Vector3(0.60309f, 0.65983f, 0.77929f);
                W = new Vector3(0.60557f, 0.96479f, 0.90710f);
                X = new Vector3(0.87701f, 0.70825f, 0.73624f);
                Y = new Vector3(0.86193f, 0.62384f, 0.77986f);

                texturaArray = new Vector3[,] { { A, B, C, D, E },
                                                { F, G, H, I, J },
                                                { K, L, M, N, O },
                                                { P, Q, R, S, T },
                                                { U, V, W, X, Y }};

                textureIdx = 1;
                break;
            case "tlacuilo":
                ka = new Vector3(0.21387f, 0.13812f, 0.26056f );
                kd = new Vector3(0.01357f, 0.83738f, 0.15432f);
                ks = new Vector3(0.36876f, 0.37432f, 0.45569f);

                Ia = new Vector3(0.92420f, 0.93733f, 0.83802f);
                Id = new Vector3(0.92900f, 0.92576f, 0.99849f);
                Is = new Vector3(0.85123f, 0.96109f, 0.97482f);

                alpha = 54;
                sphradius = 0.78628f;

                Ap = new Vector3(1.97494f, 0.99925f,-0.01874f);
                TA = new Vector3(0.02826f, 0.34906f,-0.80079f);
                Piv = new Vector3(0.58123f, 0.20356f,-0.43642f);
                axis = "X";
                rotation = 7.0f;
                inc = 43;
                azi = 100;

                LIGHT = new Vector3(3.75823f, 3.52691f,-3.20100f);
                CAMERA = new Vector3(5.49596f, 4.22602f, 4.15158f);

                dataName = "tlacuiloC";
                cameraName = "CameraC";
                lightName = "LightC";
                sphereName = "SphereC";
                A =  new Vector3(0.78951f,  0.99290f,  0.75230f);
                B =  new Vector3(0.98409f,  0.69478f,  0.72012f);
                C =  new Vector3(0.73316f,  0.80869f,  0.62864f);
                D =  new Vector3(0.67578f,  0.97587f,  0.70765f);
                E =  new Vector3(0.82473f,  0.80418f,  0.81901f);
                F =  new Vector3(0.76850f,  0.77783f,  0.79449f);
                G =  new Vector3(0.73642f,  0.73761f,  0.68937f);
                H =  new Vector3(0.98751f,  0.68209f,  0.80513f);
                I =  new Vector3(0.67562f,  0.69647f,  0.86408f);
                J =  new Vector3(0.75468f,  0.93649f,  0.92599f);
                K =  new Vector3(0.95609f,  0.65791f,  0.77201f);
                L =  new Vector3(0.67349f,  0.93077f,  0.74234f);
                M =  new Vector3(0.82599f,  0.73376f,  0.92546f);
                N =  new Vector3(0.64575f,  0.88480f,  0.80130f);
                O =  new Vector3(0.81156f,  0.96030f,  0.60266f);
                P =  new Vector3(0.83795f,  0.84780f,  0.93976f);
                Q =  new Vector3(0.77837f,  0.75994f,  0.70134f);
                R =  new Vector3(0.77569f,  0.97298f,  0.67218f);
                S =  new Vector3(0.79482f,  0.63093f,  0.71201f);
                T =  new Vector3(0.96584f,  0.89196f,  0.64538f);
                U =  new Vector3(0.82153f,  0.98385f,  0.80697f);
                V =  new Vector3(0.80026f,  0.84765f,  0.88785f);
                W =  new Vector3(0.78543f,  0.74929f,  0.82637f);
                X =  new Vector3(0.62760f,  0.75872f,  0.61197f);
                Y =  new Vector3(0.81159f,  0.77272f,  0.95221f);

                texturaArray = new Vector3[,] { { A, B, C, D, E },
                                                { F, G, H, I, J },
                                                { K, L, M, N, O },
                                                { P, Q, R, S, T },
                                                { U, V, W, X, Y }};

                textureIdx = 2;
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
        Debug.Log(dataName + " >> TextureCoordinates u: " + uv.x.ToString("F5") + ", v: " + uv.y.ToString("F5"));
        Debug.Log(dataName + " >> TexturePatch: " + texturePatch);
        Debug.Log(dataName + " >> Final color: " + finalColor.ToString("F5"));
        Debug.Log(dataName + " >> HexColor: " + hexColor);
    }
}
