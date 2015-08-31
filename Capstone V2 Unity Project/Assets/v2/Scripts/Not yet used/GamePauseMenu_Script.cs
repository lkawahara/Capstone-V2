using UnityEngine;
using System.Collections;

//Version 1
public class GamePauseMenu_Script : MonoBehaviour
{
    private GameSceneLoader gameController;

    void Awake()
    {
        if (GetComponent<GameSceneLoader>())
            gameController = GetComponent<GameSceneLoader>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisablePauseMenu();
        }

        //if(Input.GetKeyDown(KeyCode.D))
        //{
        //    //DisablePauseMenu();
        //}
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    //gameManager.fighterRight.GetComponent<Fighter_Script>().attackingEnabled = !gameManager.fighterRight.GetComponent<Fighter_Script>().attackingEnabled;
        //    //Debug.Log("AI attack toggledd:" + gameManager.fighterRight.GetComponent<Fighter_Script>().blockingEnabled);
        //}
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    //gameManager.fighterRight.GetComponent<Fighter_Script>().blockingEnabled = !gameManager.fighterRight.GetComponent<Fighter_Script>().blockingEnabled;
        //    //Debug.Log("AI attack toggledd:" + gameManager.fighterRight.GetComponent<Fighter_Script>().blockingEnabled);
        //}
    }
    void OnGUI()
    {
        int margin = 25;
        GUI.BeginGroup(new Rect(margin, margin, Screen.width - 2 * margin, Screen.height - 2 * margin));

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

        margin = 50;
        int xOffset = Screen.width / 8;
        int currYOffset = margin;
        int buttonHeight = Screen.height / 7;
        int buttonWidth = Screen.width * 3 / 4;

        //Resume button
        if (GUI.Button(new Rect(xOffset, currYOffset, buttonWidth, buttonHeight), "Resume"))
        {
            //change timeScale back to 1; disable menu; enable button
            DisablePauseMenu();
        }
        currYOffset += buttonHeight + margin;

        //Main Menu Button
        if (GUI.Button(new Rect(xOffset, currYOffset, buttonWidth, buttonHeight), "Main Menu"))
        {
            gameController.changeToScreen(GameSceneState.GameModeSelect);
        }
        currYOffset += buttonHeight + margin;

        //Character Select Button
        if (GUI.Button(new Rect(xOffset, currYOffset, buttonWidth, buttonHeight), "Character Select"))
        {
            gameController.changeToScreen(GameSceneState.CharacterSelect);
        }
        currYOffset += buttonHeight + margin;

        GUI.EndGroup();
    }

    private void DisablePauseMenu()
    {
        Time.timeScale = 1.0f;
        //enable pause button
        //GetComponent<GamePauseButton_Script>().enabled = true;
        //disable pause menu
        GetComponent<GamePauseMenu_Script>().enabled = false;
    }

    private void SwitchToMainMenu()
    {
        Time.timeScale = 1.0f;
        GetComponent<GameSceneLoader>().changeToScreen(GameSceneState.Title);
    }

}
