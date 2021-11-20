using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject sphere;
    public float mass;
    public float r;
    public float restitution; // Restitution coefficien, for bouncing
    public Vector3 cpos;   // current position
    public Vector3 prev;   // previous position
    public Color color;
    public bool colliding;

    public Vector3 forces;
    public Vector3 cannonForce;
    public float launchTime;
    public Vector3 accel;  // acceleration
    float dt;       // delta time

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetUp(Vector3 cannonForce) {
        this.launchTime = Time.realtimeSinceStartup;
        this.cannonForce = cannonForce;

        forces.y = -mass * 9.81f;
        forces.x = cannonForce.x;
        forces.z = cannonForce.z;

        Debug.Log("Fx: " + forces.x + "Fz: " + forces.z);

        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(r*2, r*2, r*2);
        sphere.transform.localPosition = cpos;
        Renderer cr = sphere.GetComponent<Renderer>();
        color = new Color(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f));
        cr.material.SetColor("_Color", color);
    }

    // The machine doesnt know when we reach the floor.
    void CollisionFloor()
    {
        // Reach floor, taking into account that the radius is touching too.
        if (cpos.y <= r)
        {
            prev.y = cpos.y;
            cpos.y = r;

            // Destroy(sphere);
            // Invert the vertical forces.
            // minus whatever the restitution is going to take
            forces.y = -forces.y * restitution;
        }
    }

    public bool CheckCollision(TankController tank)
    {
        Debug.Log("Tanbk:" +tank.centerPoint + "Cpos:" + cpos);
        float sumR = r + tank.collisionRadius;
        sumR *= sumR;
        Debug.Log("SumR:" + sumR);
        Vector3 c1 = cpos;
        Vector3 c2 = tank.centerPoint;

        float dx = c2.x - c1.x;
        dx *= dx;
        float dy = c2.y - c1.y;
        dy *= dy;
        float dz = c2.z - c1.z;
        dz *= dz;
        float d2 = dx + dy + dz;

        Debug.Log("SumR:" + sumR + "d2: " + d2);

        return sumR >= d2; // true when theres a collision
    }

    // Update is called once per frame
    void Update()
    {
        // Store current position to asign it as previous position.
        dt = Time.deltaTime; // Updating time difference.

        if (Time.frameCount > 100)
        {
            if(Mathf.Abs(cpos.y - prev.y) < 0.001f && Mathf.Abs(cpos.y - r) < 0.001f)
            {
                cpos.y = r;
                prev.y = r;
                forces.y = 0;
                // Should stop here.
            }
            else
            {
                float timeSinceLaunch = Time.realtimeSinceStartup - launchTime;
                forces.y = -mass * 9.81f;
                // forces.y += cannonForce.y - Mathf.Max(9.81f * timeSinceLaunch, 0);
                forces.y += mass * Mathf.Max(cannonForce.y - 9.81f * timeSinceLaunch, 0);

                // Atmospheric resistance
                // v = d / t
                Vector3 v = (cpos - prev) / dt;
                if (cpos.y > prev.y)
                {
                    // Going up
                    forces.y -= r * 0.0001f * v.magnitude;
                }
                else if (cpos.y < prev.y)
                {
                    // Going down
                    forces.y += r * 0.0001f * v.magnitude;

                }
            }
            Vector3 temp = cpos;
            accel = forces / mass;
            cpos = 2 * cpos - prev + accel * dt * dt; // Vercel's
            prev = temp;
            sphere.transform.position = cpos; // Move the particle here.
            CollisionFloor();
        }
    }
}

