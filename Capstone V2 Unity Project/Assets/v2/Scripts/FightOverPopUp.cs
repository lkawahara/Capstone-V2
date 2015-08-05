using UnityEngine;
using System.Collections;

public class FightOverPopUp : MonoBehaviour
{
    GameObject winner;

    void OnGUI()
    {
        int margin = 25;
        GUI.BeginGroup(new Rect(margin, margin, Screen.width - 2 * margin, Screen.height - 2 * margin));

        //Background box
        if (winner) {
            GUI.Box(new Rect(0, 20, Screen.width, Screen.height), winner.name + " Wins!");
        }
        
        margin = 50;
        int xOffset = Screen.width / 8;
        int currYOffset = margin;
        int buttonHeight = Screen.height / 7;
        int buttonWidth = Screen.width * 3 / 4;

        if (GUI.Button(new Rect(xOffset, currYOffset, buttonWidth, buttonHeight), "Rematch"))
        {
            ClickRematch();
        }
        currYOffset += buttonHeight + margin;

        if (GUI.Button(new Rect(xOffset, currYOffset, buttonWidth, buttonHeight), "Reselect Characters"))
        {
            ClickReselectCharacters();
        }
        currYOffset += buttonHeight + margin;

        if (GUI.Button(new Rect(xOffset, currYOffset, buttonWidth, buttonHeight), "Game Mode Select"))
        {
            ClickGameModeSelect();
        }
        GUI.EndGroup();
    }

    //TODO on win initialize
    public void Initialize(GameObject win)
    {
        winner = win;
        this.enabled = true;
    }

    public void ClickRematch()
    {
        GetComponent<GameSceneController>().changeToScreen(GameSceneState.Fight);
        unpause();
    }

    //not active
    public void ClickReselectCharacters()
    {
        GetComponent<GameSceneController>().changeToScreen(GameSceneState.CharacterSelect);
        unpause();
    }

    public void ClickGameModeSelect()
    {
        GetComponent<GameSceneController>().changeToScreen(GameSceneState.GameModeSelect);
        unpause();
    }

    private void unpause() {
        Time.timeScale = 1;
        this.enabled = false;
    }
}
