using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation101 : MonoBehaviour
{
    public GameObject GO_SUN;
    public GameObject GO_EARTH;
    public GameObject GO_MOON;
    public float deltaY;
    public float sunScale;
    public float distanceEarth;
    public float distanceMoon;

    Vector3[] sun_o;
    Vector3[] earth_o;
    Vector3[] moon_o;

    // Rotations per frame of GO_SUN;
    float rotY; // Declaration, its better to initialize in start()

    // Start is called before the first frame update
    void Start()
    {
        rotY = 0.0f; // Initialization.


        // We save the original position of vertices in GOs so that the transformations dont accumulate.

        Mesh sun_mesh = GO_SUN.GetComponent<MeshFilter>().mesh;
        sun_o = new Vector3[GO_SUN.GetComponent<MeshFilter>().mesh.vertices.Length];
        for (int i = 0; i < sun_o.Length; i++) {
            sun_o[i] = sun_mesh.vertices[i];
        }

        Mesh earth_mesh = GO_EARTH.GetComponent<MeshFilter>().mesh;
        earth_o = new Vector3[GO_EARTH.GetComponent<MeshFilter>().mesh.vertices.Length];
        for (int i = 0; i < earth_o.Length; i++) {
            earth_o[i] = earth_mesh.vertices[i];
        }

        Mesh moon_mesh = GO_MOON.GetComponent<MeshFilter>().mesh;
        moon_o = new Vector3[GO_MOON.GetComponent<MeshFilter>().mesh.vertices.Length];
        for (int i = 0; i < moon_o.Length; i++) {
            moon_o[i] = moon_mesh.vertices[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        rotY += deltaY; // Here rotation is aggregated.

        // SUN

        // We want to rotate around Y axis.
        // We want the angle value to change in every frame
        Matrix4x4 rotMS = Transformations.RotateM(rotY, Transformations.AXIS.AX_Y);;
        Matrix4x4 scaleS = Transformations.ScaleM(sunScale, sunScale, sunScale);

        // Transform the mesh
        Mesh mesh = GO_SUN.GetComponent<MeshFilter>().mesh;

        Vector3[] transformed = new Vector3[sun_o.Length];
        for (int i = 0; i < sun_o.Length; i++) {
            Vector3 o = sun_o[i];
            Vector4 temp = new Vector4(o.x, o.y, o.z, 1);
            transformed[i] = rotMS * scaleS * temp;
        }

        mesh.vertices = transformed;
        mesh.RecalculateNormals();

        // This is a expensive process.

        // EARTH

        Matrix4x4 scaleE = Transformations.ScaleM(sunScale * 0.2f, sunScale * 0.2f, sunScale * 0.2f);
        Matrix4x4 rotME = Transformations.RotateM(rotY*4, Transformations.AXIS.AX_Y);;
        Matrix4x4 tranME = Transformations.TranslateM(distanceEarth, 0, 0);

        // Transform the mesh of the earth
        Mesh meshE = GO_EARTH.GetComponent<MeshFilter>().mesh;

        Vector3[] transformedE = new Vector3[earth_o.Length];
        for (int i = 0; i < earth_o.Length; i++) {
            Vector3 o = earth_o[i];
            Vector4 temp = new Vector4(o.x, o.y, o.z, 1);
            transformedE[i] = rotME * tranME * scaleE * temp;
        }

        meshE.vertices = transformedE;
        meshE.RecalculateNormals();

        // MOON

        Matrix4x4 scaleM = Transformations.ScaleM(sunScale * 0.4f, sunScale * 0.4f, sunScale * 0.4f);
        Matrix4x4 rotMM = Transformations.RotateM(rotY*15, Transformations.AXIS.AX_Y);;
        Matrix4x4 tranMM = Transformations.TranslateM(distanceMoon, 0, 0);

        // Transform the mesh of the moon
        Mesh meshM = GO_MOON.GetComponent<MeshFilter>().mesh;

        Vector3[] transformedM = new Vector3[moon_o.Length];
        for (int i = 0; i < moon_o.Length; i++) {
            Vector3 o = moon_o[i];
            Vector4 temp = new Vector4(o.x, o.y, o.z, 1);
            transformedM[i] =  rotME * tranME * rotMM * tranMM * scaleM * temp;
        }

        meshM.vertices = transformedM;
        meshM.RecalculateNormals();
    }
}
