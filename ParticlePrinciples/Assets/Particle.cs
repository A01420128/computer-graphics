using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public float mass;
    // public float r;        // radius
    Vector3 cpos;   // current position
    Vector3 prev;   // previous position
    // public Vector3 vel;    // velocity
    public Vector3 forces;
    public Vector3 accel;  // acceleration

    float dt;       // delta time

    // Start is called before the first frame update
    void Start()
    {
        cpos = transform.localPosition;
        prev = cpos;
        forces.y = mass * -9.81f; // Gravity is the only acting force.
    }

    // Update is called once per frame
    void Update()
    {
        // Store current position to asign it as previous position.
        Vector3 temp = cpos;
        dt = Time.deltaTime; // Updating time difference.
        // Wait 100 frames before starting
        if (Time.frameCount > 100)
        {
            accel = forces / mass;
            cpos = 2 * cpos - prev + accel * dt * dt;
            prev = temp;
        }

        transform.localPosition = cpos; // Move the particle here.
    }
}
