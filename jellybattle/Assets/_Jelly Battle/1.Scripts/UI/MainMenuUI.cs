using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject progressPanel;
    public GameObject controlPanel;
    public GameObject connectingLabel;
    public GameObject quitButton;

    public void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Launcher.instance.QuitGame();
        }
    }
}
