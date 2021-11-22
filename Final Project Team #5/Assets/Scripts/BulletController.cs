// FINAL PROJECT
//
// Javier Flores
// Enrique Orduna
// Jose Tlacuilo
//

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
    public int timesBounced;
    private bool hasDamage;

    public Vector3 forces;
    public Vector3 cannonForce;
    public float launchTime;
    public Vector3 accel;  // acceleration
    float dt;       // delta time

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetUp(Vector3 cannonForce, Color color) {
        this.launchTime = Time.realtimeSinceStartup;
        this.cannonForce = cannonForce;
        this.timesBounced = 0;
        this.hasDamage = true;

        forces.y = -mass * 9.81f;
        forces.x = cannonForce.x;
        forces.z = cannonForce.z;

        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(r*2, r*2, r*2);
        sphere.transform.localPosition = cpos;
        Renderer cr = sphere.GetComponent<Renderer>();
        this.color = color;
        cr.material.SetColor("_Color", this.color);
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
            timesBounced += 1;
        }
    }

    public bool CheckCollision(TankController tank)
    {
        if (!hasDamage)
        {
            return false;
        }
        float sumR = r + tank.collisionRadius;
        sumR *= sumR;
        Vector3 c1 = cpos;
        Vector3 c2 = tank.centerPoint;

        float dx = c2.x - c1.x;
        dx *= dx;
        float dy = c2.y - c1.y;
        dy *= dy;
        float dz = c2.z - c1.z;
        dz *= dz;
        float d2 = dx + dy + dz;

        bool hit = sumR >= d2;

        if (hit)
        {
            Debug.Log("Bounced");
            hasDamage = false;
            BounceFrom(tank, cpos);
        }

        return hit;
    }

    void BounceFrom(TankController tank, Vector3 cpos)
    {
        Vector3 normal = cpos - tank.centerPoint;
        Debug.Log("TANK_CENTER: " + tank.centerPoint.ToString("F5"));
        Debug.Log("COLLISION_POINT: " + cpos.ToString("F5"));
        Debug.Log("NORMAL: " + normal.ToString("F5"));
        Vector3 refletedRay = Mathematics.Reflect(forces, normal);
        Debug.Log("FORCES: " + forces.ToString("F5"));
        Debug.Log("REFLECTED: " + refletedRay.ToString("F5"));
        forces = refletedRay * 8;
        Debug.Log("FORCES: " + forces.ToString("F5"));
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

