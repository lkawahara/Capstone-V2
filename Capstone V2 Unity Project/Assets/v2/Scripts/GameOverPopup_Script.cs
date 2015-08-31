using UnityEngine;
using System.Collections;

//Version 2
public class GameOverPopup_Script : MonoBehaviour
{
    private GameObject winner;
    private FightStateManager fsm;
    private GameSceneLoader scm;

    public void Awake() {
        fsm = GetComponent<FightStateManager>();
        scm = GetComponent<GameSceneLoader>();
    }

    void OnGUI()
    {
        int margin = 25;
        GUI.BeginGroup(new Rect(margin, margin, Screen.width - 2 * margin, Screen.height - 2 * margin));
        
        //Background box
        GUI.Box(new Rect(0, 20, Screen.width, Screen.height), winner.name + " Wins!");
        
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

        //if (GUI.Button(new Rect(xOffset, currYOffset, buttonWidth, buttonHeight), "Reselect Characters"))
        //{
        //    //ClickReselectCharacters();
        //}
        //currYOffset += buttonHeight + margin;

        if (GUI.Button(new Rect(xOffset, currYOffset, buttonWidth, buttonHeight), "Main Menu"))
        {
            ClickMainMenu();
        }
        GUI.EndGroup();
    }

    public void Initialize(GameObject win)
    {
        winner = win;
        this.enabled = true;
    }

    //gameOverComponent
    public void ClickRematch()
    {
        fsm.Rematch();
        unpauseAndDisable();
    }

    //not active
    public void ClickReselectCharacters()
    {
        scm.changeToScreen(GameSceneState.CharacterSelect);
        unpauseAndDisable();
    }

    public void ClickMainMenu()
    {
        scm.changeToScreen(GameSceneState.Title);
        unpauseAndDisable();
    }

    private void unpauseAndDisable() {
        Time.timeScale = 1;
        this.enabled = false;
    }
}
