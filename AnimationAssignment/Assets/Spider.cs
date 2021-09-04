// Computer grafics
// September 4, 2021
// Animation Assignment Spider

// Javier Flores - A01651678
// Enrique Orduna - A01027318
// Jose Javier Tlacuilo - A01420128

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    public float legSpeed;

    Leg legHFL;
    Leg legHFR;
    Leg legHBL;
    Leg legHBR;

    Leg legBFL;
    Leg legBFR;
    Leg legBBL;
    Leg legBBR;
    BodySphere body;

    // Start is called before the first frame update
    void Start()
    {
        body = new BodySphere();
        // Leg(float sAngle, float deltaRotZ, float width, float length, Vector3 position, int posAngle, bool isForward)
        legHFL = new Leg(30f, 0.125f, 0.7f, new Vector3(0.30f, 0, 0.625f), -45, false);
        legHFR = new Leg(30f, 0.125f, 0.7f, new Vector3(-0.30f, 0, 0.625f), -135, true);

        legHBL = new Leg(30f, 0.125f, 0.7f, new Vector3(0.30f, 0, 0.375f), -10, true);
        legHBR = new Leg(30f, 0.125f, 0.7f, new Vector3(-0.30f, 0, 0.375f), -170, false);

        legBFL = new Leg(30f, 0.125f, 0.9f, new Vector3(0.30f, 0, 0.125f), 10, false);
        legBFR = new Leg(30f, 0.125f, 0.9f, new Vector3(-0.30f, 0, 0.125f), 170, true);

        legBBL = new Leg(30f, 0.125f, 0.9f, new Vector3(0.30f, 0, -0.125f), 45, true);
        legBBR = new Leg(30f, 0.125f, 0.9f, new Vector3(-0.30f, 0, -0.125f), 135, false);
    }

    // Update is called once per frame
    void Update()
    {
        legHFL.Move(legSpeed);
        legHFR.Move(legSpeed);
        legHBL.Move(legSpeed);
        legHBR.Move(legSpeed);

        legBFL.Move(legSpeed);
        legBFR.Move(legSpeed);
        legBBL.Move(legSpeed);
        legBBR.Move(legSpeed);
    }
}
