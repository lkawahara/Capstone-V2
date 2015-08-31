using UnityEngine;

//Version 2
//attach to the player it is managing
//only displays health, doesnt actually manage

//health display
public class HealthManager_Script : MonoBehaviour {

    //textures
    public Texture2D healthFillTexture;
    public Texture2D healthFrameTexture;
    
    //sizing sliders
    public Vector2 frameMargin;
    public Vector2 frameSize; //in percentage
    public Vector2 healthMargin;
    public Vector2 healthSize;

    private Fighter_Script fighter;
    private Vector2 resizeVar;
    private float healthPercentage;
    private const float minHealthPercentage = 0.001f; // constrain the bar to the frame
    private const float maxHealthPercentage = 1.0f;

    void Awake()
    {
        if (!healthFrameTexture)
        {
            Debug.LogError("HealthManager: Assign the Healthbar Frame texture");
        }
        if (!healthFillTexture)
        {
            Debug.LogError("HealthManager: Assign the Healthbar Fill texture");
        }
        if (!GetComponent<Fighter_Script>())
        {
            Debug.LogError("HealthManager: Managing a fighter with no fighter script attached");
        }
        fighter = GetComponent<Fighter_Script>();
    }

    void Update()
    {
        healthPercentage = fighter.GetHealthPercentage();
    }

    void OnGUI()
    {
        //placement on screen is determined by a tag
        if (gameObject.tag == "Player")
        {
            //healthFill
            resizeVar.x = Screen.width * healthSize.x;
            resizeVar.y = Screen.height * healthSize.y;
            GUI.DrawTexture(new Rect(healthMargin.x, healthMargin.y, healthPercentage * resizeVar.x, resizeVar.y), healthFillTexture, ScaleMode.StretchToFill, true, 0);

            //healthFrame
            resizeVar.x = Screen.width * frameSize.x;
            resizeVar.y = Screen.height * frameSize.y;
            GUI.DrawTexture(new Rect(frameMargin.x, frameMargin.y, resizeVar.x, resizeVar.y), healthFrameTexture, ScaleMode.StretchToFill, true, 0);
        }
        else 
        {
            //healthFill
            resizeVar.x = Screen.width * healthSize.x;
            resizeVar.y = Screen.height * healthSize.y;
            float xBuffer = Screen.width / 5.5f;
            GUI.DrawTexture(new Rect(Screen.width * frameSize.x + xBuffer, healthMargin.y, healthPercentage * resizeVar.x, resizeVar.y), healthFillTexture, ScaleMode.StretchToFill, true, 0);

            //healthFrame
            resizeVar.x = Screen.width * frameSize.x;
            resizeVar.y = Screen.height * frameSize.y;
            GUI.DrawTexture(new Rect(Screen.width * frameSize.x + xBuffer, frameMargin.y, resizeVar.x, resizeVar.y), healthFrameTexture, ScaleMode.StretchToFill, true, 0);
        }
    }
}