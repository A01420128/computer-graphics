// FINAL PROJECT
//
// Javier Flores
// Enrique Orduna
// Jose Tlacuilo
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public InputField player1;
    public InputField player2;
    public InputField player3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeToGame(string sceneName){
        PlayerInfo.SetPlayers(player1.text, player2.text, player3.text);
        SceneManager.LoadScene(sceneName);
    }
}
