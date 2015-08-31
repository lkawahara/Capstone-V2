using UnityEngine;
using System.Collections;

public class GamePauseButton_Script : MonoBehaviour
{
    void OnGUI()
    {
        int windowWidth = Screen.width / 4;
        int windowHeight = Screen.height / 5;
        //if (GUI.Button(new Rect(Screen.width - (windowWidth + buffer), 20, windowWidth, windowHeight), "Pause"))
            if (GUI.Button(new Rect(0, 0, windowWidth, windowHeight), "Pause"))
            {
            Time.timeScale = 0;
            GetComponent<GamePauseMenu_Script>().enabled = true;
            GetComponent<GamePauseButton_Script>().enabled = false;
        }
    }
}
