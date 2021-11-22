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

public class EnterGame : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeToSelection(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
