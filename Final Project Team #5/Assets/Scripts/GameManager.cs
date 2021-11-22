// FINAL PROJECT
//
// Javier Flores
// Enrique Orduna
// Jose Tlacuilo
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Vector3 START_POS_1 = new Vector3(0, 0, 20);
    private float START_ROT_1 = 180;
    private Vector3 START_POS_2 = new Vector3(-20, 0, -20);
    private float START_ROT_2 = 60;
    private Vector3 START_POS_3 = new Vector3(20, 0, -20);
    private float START_ROT_3 = -60;
    private float WIND_RANGE = 0.1f;

    public Text playerName;
    public Text health;
    public Text position;
    public Text rotationY;
    public Text weaponType;
    public Text cannonInfo;
    public Text windX;
    public Text windZ;
    public Text playersInfo;
    public Text tankInfo;

    public TankController player1;
    public TankController player2;
    public TankController player3;
    public GameObject terrain;

    public TankController currentPlayer;
    public List<TankController> oponents;

    private Camera mainCamera;

    private int turn;

    // Choose a random player order, available positions 0, 1, 2
    private void SetPlayerOrder()
    {
        List<int> picks = new List<int> {0,1,2};
        int firstPick = Random.Range(0,3); // random 0, 1 or 2
        int first = picks[firstPick]; // Get selected position and remove it from list
        picks.RemoveAt(firstPick); // List has two elements now.
        int secondPick = Random.Range(0,2); // random 0, 1
        int second = picks[secondPick]; // Get selected position and remove it from list
        picks.RemoveAt(secondPick); // List has one element now.
        int third = picks[0];// Get remaining position

        // Assign players to their random choosen position
        TankController[] playOrder = new TankController[3];
        playOrder[first] = player1;
        player1.playerName = PlayerInfo.players[0].name;
        player1.health = 100;
        playOrder[second] = player2;
        player2.playerName = PlayerInfo.players[1].name;
        player2.health = 100;
        playOrder[third] = player3;
        player3.playerName = PlayerInfo.players[2].name;
        player3.health = 100;

        currentPlayer = playOrder[0];
        currentPlayer.isPlaying = true;
        // Add wind in first turn
        currentPlayer.windX = Random.Range(-WIND_RANGE, WIND_RANGE) * 4.0f;
        currentPlayer.windZ = Random.Range(-WIND_RANGE, WIND_RANGE) * 4.0f;
        oponents.Add(playOrder[1]);
        oponents.Add(playOrder[2]);
    }

    void SetupTerrain()
    {
        Matrix4x4 st = Transformations.ScaleM(20, 20, 20);
        Mesh mt = terrain.GetComponent<MeshFilter>().mesh;
        Vector3[] transform = new Vector3[mt.vertices.Length];
        for (int i = 0; i < mt.vertices.Length; i++)
        {
            Vector3 v = mt.vertices[i];
            Vector4 temp = new Vector4(v.x, v.y, v.z, 1);
            transform[i] = st * temp;
        }
        mt.vertices = transform;
        mt.RecalculateBounds();
    }

    private void DisplayPlayerInfo()
    {
        playerName.text = "Player name:\n" + currentPlayer.playerName;
        health.text = "Health: " + currentPlayer.health;
        position.text = "Position:\nX: " + currentPlayer.centerPoint.x.ToString("F") + ", Z: " + currentPlayer.centerPoint.z.ToString("F");
        rotationY.text = "Y-Rotation:\n" + currentPlayer.tankYRotation;
        WeaponType w = currentPlayer.currentWeapon;
        weaponType.text = "Weapon: " + w.type + "\ndmg: " + w.damage + " pow: " + w.velocity + " ammo: " + w.ammo;
        cannonInfo.text = "Cannon:\nVelocity: " + currentPlayer.cannonVelocity + " Angle: " + currentPlayer.cannonAngle;
        windX.text = "Wind-X:\n" + (currentPlayer.windX * 10.0f).ToString("F");
        windZ.text = "Wind-Z:\n" + (currentPlayer.windZ * 10.0f).ToString("F");
        string oponentsStr = "";
        foreach(TankController o in oponents)
        {
            oponentsStr += "\n" + o.playerName + ": " + o.health;
        }
        playersInfo.text = "Players health:\n" + currentPlayer.playerName + ": " + currentPlayer.health + oponentsStr;
        tankInfo.text = currentPlayer.tankInfo;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>(); 
        oponents = new List<TankController>();
        SetupTerrain();
        SetPlayerOrder();
        player1.SetUp(START_POS_1, START_ROT_1);
        player2.SetUp(START_POS_2, START_ROT_2);
        player3.SetUp(START_POS_3, START_ROT_3);
        mainCamera.transform.position = currentPlayer.cameraPoint;
        mainCamera.transform.LookAt(currentPlayer.cannonPoint);
        DisplayPlayerInfo();
    }

    // Update is called once per frame
    void Update()
    {
        DisplayPlayerInfo();

        // Check if current player has hit a tank or bullet has bounced more than once.
        if (currentPlayer.CheckHits(oponents))
        {
            // Check how many alive
            for (int i = 0; i < oponents.Count; i++)
            {
                if (oponents[i].health <= 0)
                {
                    Debug.Log("Oponent died: " + oponents[i].playerName);
                    MeshRenderer m = oponents[i].GetComponent<MeshRenderer>();
                    m.enabled = false;
                    oponents.RemoveAt(i);
                }
            }

            // Checks if someone won.
            if (oponents.Count == 0)
            {
                Debug.Log("Gano: " + currentPlayer.playerName);
                currentPlayer.tankInfo = "Congratulations you won!!\n" + currentPlayer.playerName;
                currentPlayer.isPlaying = false;
            } else {
                // Else change oponents and current player
                oponents.Add(currentPlayer); // Add current player to the end of the list.
                currentPlayer = oponents[0]; // The new current player will be the first of the oponents
                currentPlayer.isPlaying = true;
                oponents.RemoveAt(0); // The first oponents  is now the current player so its not an oponent anymore.

                Debug.Log("Number of oponents: " + oponents.Count);

                foreach(TankController oponent in oponents)
                {
                    oponent.isPlaying = false;
                }

                mainCamera.transform.position = currentPlayer.cameraPoint;
                mainCamera.transform.LookAt(currentPlayer.cannonPoint);

                // Add wind in turn
                currentPlayer.windX = Random.Range(-WIND_RANGE, WIND_RANGE) * 4.0f;
                currentPlayer.windZ = Random.Range(-WIND_RANGE, WIND_RANGE) * 4.0f;
            }
        }
    }
}
