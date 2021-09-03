using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    Leg leg;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Create body, two spheres.
        leg = new Leg(30);
    }

    // Update is called once per frame
    void Update()
    {
        leg.Move();
    }
}
