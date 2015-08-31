using UnityEngine;
using System.Collections;

//Version 2
//game scene loads (fades in)
//show both fighters with a cutscene of some sort
//FIGHT!! (Plane texture moving in and out of the screen)
//start timer
public class FightStateManager : MonoBehaviour
{
#region vars
    //user changeable vars
    private int roundTimeLimit = 60;
    public GameObject screenFader;
    public GameObject fighterLeft;
    public GameObject fighterRight;
    public Texture2D[] roundTextures; //textures to be shown to player when rounds start
    public Texture2D fightStartTexture;
    public GameObject notificationBanner; //used for: round number, "fight!!"
    
    //debug
    public bool debugOn = false; //used to show debug button to advance game state
    public bool manageFighterPosition = true;

    //backend
    private float notificationBannerVisibleTime = 1.0f; //how long to keep notification banner on screen
    private float mapMinZ = -10.0f;
    private float mapMaxZ = 10.0f;
    private float fighterLeftStartZ = -5.0f;
    private float fighterRightStartZ = 5.0f;
    private float buffer = 3.0f;
    private GameObject focusFighter; //used for camera tricks 

    public enum GameState
    {
        nullState,
        FighterIntro,
        RoundStart,
        GamePlay,
        RoundEnd,
        GameOver,
        Error
    };

    private GameState currentState;
    private float roundTimer;
    private float introTimer;
    private int currRound = 1; //used for determining which texture to display
#endregion

#region setUp
    void Awake()
    {
        GetComponent<GUITexture>().pixelInset = new Rect(0, 0, Screen.width, Screen.height);
        setCurrentNotificationTexture(roundTextures[currRound]);
    }

    /// <summary>
    /// Sets focusFighter, updates fighter positions to starting positions
    /// </summary>
    void Start()
    {
        currentState = GameState.RoundStart;
        focusFighter = fighterLeft;
        fighterLeft.gameObject.transform.position = new Vector3(0, 0, fighterLeftStartZ);
        fighterRight.gameObject.transform.position = new Vector3(0, 0, fighterRightStartZ);
        roundTimer = roundTimeLimit;
    }


    /// <summary>
    /// Resets entire Match
    /// Calls RoundStartSetUp()
    /// (texture to currRound, active)
    /// (currState)
    /// (timers: introTimer, roundTimer)
    /// (fighter: health, hurtcount, lastInput, animatorParams) 
    ///(fighter : position)
    /// </summary>
    private void MatchStartSetUp()
    {
        RoundStartSetUp();
        fighterLeft.GetComponent<Fighter_Script>().MatchReset(); //todo calls fighter.roundReset twice
        fighterRight.GetComponent<Fighter_Script>().MatchReset();

        fighterLeft.gameObject.transform.position = new Vector3(0, 0, fighterLeftStartZ);
        fighterRight.gameObject.transform.position = new Vector3(0, 0, fighterRightStartZ);
    }

    /// <summary>
    /// Resets round 
    /// (texture to currRound, active)
    /// (currState = RoundStart)
    /// (timers: introTimer, roundTimer)
    /// (fighter: health, hurtcount, lastInput, animatorParams) 
    /// </summary>
    private void RoundStartSetUp()
    {
        //set guitexture to display the round number
        Texture2D currRoundTexture = roundTextures[0];
        switch (currRound)
        {
            case 2: { currRoundTexture = roundTextures[1]; break; }
            case 3: { currRoundTexture = roundTextures[2]; break; }
            default: { break; }
        }
        setCurrentNotificationTexture(currRoundTexture);
        notificationBanner.SetActive(true);

        currentState = GameState.RoundStart;

        introTimer = notificationBannerVisibleTime;//set to the interval time: used for keeping the round/fight notification on screen for a period of time //roundTimeLimit;
        roundTimer = roundTimeLimit;

        Debug.Log("Players Reset");
        fighterLeft.GetComponent<Fighter_Script>().RoundReset();
        fighterRight.GetComponent<Fighter_Script>().RoundReset();
    }

    /// <summary>
    /// Resets game state to start of match, called by gameoverpopup
    /// </summary>
    public void Rematch()
    {
        //when rematch is chosen the characters aren't introduced anymore
        currRound = 1;
        MatchStartSetUp();
    }
    #endregion

#region update
    void Update()
    {
        if (manageFighterPosition)
            ManageFighterZPosition();
        //Pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            GetComponent<GamePauseMenu_Script>().enabled = true;
            //GetComponent<GamePauseButton_Script>().enabled = false;
        }
        updateGameState();
    }

    private void updateGameState() {
        switch (currentState)
        {
            //TODO check if fader works
            case GameState.nullState:
                {
                    Debug.Log("Fading in");
                    if (!screenFader.GetComponent<ScreenFader>().isFading())
                    {
                        currentState = GameState.FighterIntro;
                    }
                    break;
                }
            case GameState.FighterIntro:
                {
                    FighterIntroUpdate();
                    break;
                }
            case GameState.RoundStart:
                {
                    RoundStartNotificationUpdate();
                    break;
                }
            case GameState.GamePlay:
                {
                    GamePlayUpdate();
                    break;
                }
            case GameState.RoundEnd:
                {
                    RoundEndUpdate();
                    break;
                }
            case GameState.GameOver:
                {
                    //should not worry about multiple updates because timescale is set to 0 on change to Gameover
                    GameOverUpdate();
                    break;
                }
            case GameState.Error:
                //used as default as well as handling errors
                {
                    break;
                }
        }
    }

    /// <summary>
    /// Advances a frame in the fighterIntroductions
    /// once the fighters are introduced, changes gamestate
    /// TODO focuses camera on focusFighter
    /// </summary>
    private void FighterIntroUpdate()
    {
        //cam.transform.LookAt(focusFighter.transform);
        if (!focusFighter.GetComponent<Fighter_Script>().IsIntroduced())
        {
            //should hit here when the fighter's introduced boolean is toggled to true from start, is toggled from animation event from the player himself
            focusFighter.GetComponent<Fighter_Script>().IntroduceFighter();
        }
        else
        {
            AdvanceGameState();
        }
    }

    /// <summary>
    /// when this is called the round display should already be showing
    /// stays for a time 
    /// then change to "fight !!" texture
    /// stays for a time
    /// disappear and switch to game play
    /// </summary>
    private void RoundStartNotificationUpdate()
    {
        introTimer -= Time.deltaTime;
        if (introTimer <= 0)
        {
            if (getCurrentNotificationTexture() != fightStartTexture)
            {
                //round notification complete, change to fight start notification
                setCurrentNotificationTexture(fightStartTexture);
                introTimer = notificationBannerVisibleTime;
            }
            else
            {
                notificationBanner.SetActive(false);
                AdvanceGameState();
            }
        }
    }

    /// <summary>
    /// Waits for both fighters to go back to idle animation, then advances game state
    /// TODO include a check, when the player is near death if an incoming attack would kill the player slow down the hit impact as the player's health goes down
    /// TODO have the health decrease at a constant rate instead of instantaniously decreasing when damaged
    /// </summary>
    private void RoundEndUpdate()
    {
        if (fighterLeft.GetComponent<Fighter_Script>().isIdle() && fighterRight.GetComponent<Fighter_Script>().isIdle())
        {
            Debug.Log("Win Die animations Complete");
            AdvanceGameState();
        }
    }

    /// <summary>
    /// Updates round timer, Advances game state if time limit is reached or a player dies
    /// </summary>
    private void GamePlayUpdate()
    {
        roundTimer -= Time.deltaTime;
        //if either fighter is dead
        if (roundTimer <= 0 ||
            fighterLeft.GetComponent<Fighter_Script>().IsDead() ||
            fighterRight.GetComponent<Fighter_Script>().IsDead())
        {
            AdvanceGameState();
        }
    }

    /// <summary>
    /// Pause game, enable gameoverpopup script passing in winning fighter game object
    /// </summary>
    private void GameOverUpdate()
    {
        Time.timeScale = 0;
        GameObject winner = (fighterRight.GetComponent<Fighter_Script>().GetNumWins() > fighterLeft.GetComponent<Fighter_Script>().GetNumWins()) ? fighterRight : fighterLeft;
        GetComponent<GameOverPopup_Script>().Initialize(winner);
    }

    public void AdvanceGameState()
    {
        GameState temp = currentState;
        switch (temp)
        {
            case GameState.FighterIntro:
                {
                    focusFighter = (focusFighter == fighterLeft) ? fighterRight : fighterLeft;
                    //if both are introduced
                    if (fighterLeft.GetComponent<Fighter_Script>().IsIntroduced()
                        && fighterRight.GetComponent<Fighter_Script>().IsIntroduced())
                    {
                        //turn controls on for characters
                        //Todo change camera look at back to origin
                        MatchStartSetUp();
                    }
                    break;
                }
            case GameState.RoundStart:
                {
                    fighterLeft.GetComponent<Fighter_Script>().SetActive(true);
                    fighterRight.GetComponent<Fighter_Script>().SetActive(true);
                    currentState = GameState.GamePlay;
                    break;
                }
            case GameState.GamePlay:
                {
                    Debug.Log("Round Over");

                    currentState = GameState.RoundEnd;
                    //change animation state of players
                    if (fighterLeft.GetComponent<Fighter_Script>().GetHealthPercentage() > fighterRight.GetComponent<Fighter_Script>().GetHealthPercentage())
                    {
                        Debug.Log("FighterLeft wins");
                        fighterLeft.GetComponent<Fighter_Script>().WinLoseAnimation(true);
                        fighterRight.GetComponent<Fighter_Script>().WinLoseAnimation(false);
                    }
                    else
                    {
                        Debug.Log("FighterRight wins");
                        fighterLeft.GetComponent<Fighter_Script>().WinLoseAnimation(false);
                        fighterRight.GetComponent<Fighter_Script>().WinLoseAnimation(true);
                    }
                    break;
                }
            case GameState.RoundEnd:
                {
                    Debug.Log("Left Wins: " + fighterLeft.GetComponent<Fighter_Script>().GetNumWins());
                    Debug.Log("Right Wins: " + fighterRight.GetComponent<Fighter_Script>().GetNumWins());
                    if (fighterLeft.GetComponent<Fighter_Script>().GetNumWins() >= 2
                        || fighterRight.GetComponent<Fighter_Script>().GetNumWins() >= 2)
                    {
                        Debug.Log("Switching to GameOver State");
                        currentState = GameState.GameOver;
                    }
                    else
                    {
                        currRound++;
                        Debug.Log("Starting Round " + currRound.ToString());
                        RoundStartSetUp();
                    }
                    break;
                }
            case GameState.GameOver:
                {
                    Debug.LogError("Trying to advance from the game over state");
                    break;
                }
            case GameState.Error:
                {
                    Debug.LogError("Trying to advance from the error state");
                    break;
                }
        }
    }
#endregion

#region fighter management
    //fighters can pass through each other but will rotate accordingly to face each other; currently broken because the input 
    private void ManageOrientation()
    {
        if (fighterLeft.transform.position.z < fighterRight.transform.position.z)
        {
            Debug.Log("Left player on left");
            //change fighter left direction facing
            Vector3 newRotation = fighterLeft.transform.rotation.eulerAngles;
            newRotation.y = 0;
            fighterLeft.transform.rotation = Quaternion.Euler(newRotation);

            //change fighter right direction facing
            newRotation = fighterRight.transform.rotation.eulerAngles;
            newRotation.y = 180;
            fighterRight.transform.rotation = Quaternion.Euler(newRotation);
        }
        else if (fighterLeft.transform.position.z > fighterRight.transform.position.z)
        {
            Debug.Log("Left player on Right");
            //change fighter left direction facing
            Vector3 newRotation = fighterLeft.transform.rotation.eulerAngles;
            newRotation.y = 180;
            fighterLeft.transform.rotation = Quaternion.Euler(newRotation);

            //change fighter right direction facing
            newRotation = fighterRight.transform.rotation.eulerAngles;
            newRotation.y = 0;
            fighterRight.transform.rotation = Quaternion.Euler(newRotation);
        }
    }

    /// <summary>
    /// Manages fighter's positions so they can't back up off screen, or go past the other fighter
    /// </summary>
    private void ManageFighterZPosition()
    {
        float fighterLeftMaxZ = fighterRight.gameObject.transform.position.z - buffer;
        Vector3 fighterLeftPosition = fighterLeft.transform.position;
        fighterLeftPosition.z = Mathf.Clamp(fighterLeftPosition.z, mapMinZ, fighterLeftMaxZ);
        fighterLeft.transform.position = fighterLeftPosition;

        float fighterRightMinZ = fighterLeft.gameObject.transform.position.z + buffer;
        Vector3 fighterRightPosition = fighterRight.transform.position;
        fighterRightPosition.z = Mathf.Clamp(fighterRightPosition.z, fighterRightMinZ, mapMaxZ);
        fighterRight.transform.position = fighterRightPosition;
    }
    #endregion

#region Helpers
    private Texture getCurrentNotificationTexture() {
        return notificationBanner.GetComponent<Renderer>().material.mainTexture;
    }

    private void setCurrentNotificationTexture(Texture t) {
        notificationBanner.GetComponent<Renderer>().material.mainTexture = t;

    }
#endregion

#region Display
    void OnGUI()
    {
        float buttonW = Screen.width / 5.0f;
        float buttonH = Screen.height / 10.0f;
        float yIncrement = 70;
        //Debug.Log("Screen Width: " + Screen.width);
        float xPos = (Screen.width / 2.0f) - (Screen.width / 30.0f);
        float yPos = 10;
        displayRoundTimer(xPos, yPos, buttonW, buttonH);

        if (debugOn)
        {
            yPos += yIncrement;
            if (GUI.Button(new Rect(xPos, yPos, buttonW, buttonH), "Advance Game State"))
            {
                AdvanceGameState();
            }
        }
    }

    private void displayRoundTimer(float x, float y, float buttonW, float buttonH)
    {
        GUI.Box(new Rect(x, y, buttonW, buttonH), roundTimer.ToString("0"));
    }
    #endregion
}