using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurParticleSystem : MonoBehaviour
{
    public int numParticles;
    List<Particle> particles;

    Camera auxCam;
    Camera VFCCam;

    float ROTATION_STEP = 10.0f;
    float farD;

    void PerformVFC()
    {
        float nearD = VFCCam.nearClipPlane;
        farD = VFCCam.farClipPlane;
        Vector3 CAMERA = VFCCam.transform.localPosition;
        // float h = VFCCam.pixelHeight;
        // float w = VFCCam.pixelWidth;
        //  W: 474, 546 not fitting to camera fustrum in scene.

        // Calculatig fustrum size at farD distance
        Vector3[] corners = new Vector3[4];
        VFCCam.CalculateFrustumCorners( new Rect( 0, 0, 1, 1 ), farD, Camera.MonoOrStereoscopicEye.Mono, corners);
        float heightFustrum = ( corners[0] - corners[1] ).magnitude;
        float widthFustrum = ( corners[1] - corners[2] ).magnitude;

        float w = widthFustrum;
        float h = heightFustrum;

        Vector3 CAMxu = VFCCam.transform.right;
        Vector3 CAMyu = VFCCam.transform.up;
        Vector3 CAMzu = VFCCam.transform.forward;

        foreach (Particle p in particles)
        {
            Vector3 point = p.sphere.transform.localPosition;
            p.sphere.GetComponent<MeshRenderer>().enabled =  isInside(CAMERA, CAMxu, CAMyu, CAMzu, nearD, farD, w, h, point);
        }
    }

    bool isInside(Vector3 CAMERA, Vector3 xu, Vector3 yu, Vector3 zu, float nearD, float farD, float w, float h, Vector3 POINT)
    {
        Vector3 WECTOR = POINT - CAMERA;

        // Depth test
        float depth = Mathematics.Dot(WECTOR, zu);
        if (depth < nearD || depth > farD) { return false; }

        // Height test
        float height = Mathematics.Dot(WECTOR, yu);
        if (height < -h/2 || height > h/2) { return false; }

        // Width test
        float width = Mathematics.Dot(WECTOR, xu);
        if (width < -w/2 || width > w/2) { return false; }

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Camera setup
        auxCam = GameObject.Find("Aux Camera").GetComponent<Camera>();
        VFCCam = GameObject.Find("Main Camera").GetComponent<Camera>();

        auxCam.transform.position = new Vector3(0, 5f, -10.0f);
        VFCCam.transform.position = new Vector3(0, 5f, 0);
        farD = VFCCam.farClipPlane;
        Debug.Log(farD);

        // Aux cam is the one viewing the scene.
        auxCam.enabled = true;
        VFCCam.enabled = false;

        auxCam.transform.LookAt(VFCCam.transform);

        // Instancing the scripts.
        particles = new List<Particle>(numParticles);
        for(int i = 0; i < numParticles; i++)
        {
            particles.Add(gameObject.AddComponent<Particle>());
        }

        // Asigning random properties to the particles
        foreach(Particle p in particles)
        {
            p.mass = 10.0f;
            p.r = Random.Range(0.5f, 2.0f);
            p.restitution = 0.9f;
            p.cpos = new Vector3(Random.Range(-farD, farD), VFCCam.transform.localPosition.y, Random.Range(-farD, farD));
            p.prev = p.cpos;
            p.colliding = false;
            p.SetUp();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount > 100)
        {
            Vector3 camRotation = new Vector3(0, ROTATION_STEP, 0);
            if (Input.GetKeyDown(KeyCode.A))
            {
                VFCCam.transform.eulerAngles = VFCCam.transform.eulerAngles - camRotation;
            } else if (Input.GetKeyDown(KeyCode.D))
            {
                VFCCam.transform.eulerAngles = VFCCam.transform.eulerAngles + camRotation;
            }
            PerformVFC();
        }
    }
}
