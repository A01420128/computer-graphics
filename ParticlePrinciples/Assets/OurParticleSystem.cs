using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurParticleSystem : MonoBehaviour
{
    public int numParticles;
    List<Particle> particles;

    // Start is called before the first frame update
    void Start()
    {
        // Instancing the scripts.
        particles = new List<Particle>(numParticles);
        for(int i = 0; i < numParticles; i++)
        {
            particles.Add(gameObject.AddComponent<Particle>());
        }

        foreach(Particle p in particles)
        {
            p.mass = 10.0f;
            p.r = Random.Range(0.5f, 2.0f);
            p.restitution = 0.9f;
            p.cpos = new Vector3(0, 10, 0);
            p.prev = p.cpos;
            p.colliding = false;
            p.SetUp();
        }
    }

    void CheckCollisions()
    {
        for (int p = 0; p < particles.Count; p++)
        {
            bool crashed = false;
            for (int q = p+1; q < particles.Count; q++)
            {
                if (particles[p].CheckCollision(particles[q]))
                {
                    crashed = true;
                    if (!particles[p].colliding) particles[p].sphere.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    if (!particles[q].colliding) particles[q].sphere.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    particles[p].colliding = true;
                    particles[q].colliding = true;
                }
            }
            if (!crashed)
            {
                if (particles[p].colliding)
                {
                    particles[p].sphere.GetComponent<Renderer>().material.SetColor("_Color", particles[p].color);
                    particles[p].colliding = false;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollisions();
    }
}
