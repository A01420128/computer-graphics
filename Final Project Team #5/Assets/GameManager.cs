using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TankController firstTank;
    public TankController secondTank;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Es el turno del primero
        // firstTank.checkHits(TankController[] oponents);
        firstTank.isPlaying = true;
        secondTank.isPlaying = false;
        TankController[] oponents = { secondTank };
        firstTank.CheckHits(oponents);
    }
}
