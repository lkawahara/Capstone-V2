using UnityEngine;
using System.Collections;

public class KeyBoardInputController : MonoBehaviour {

    private static string HORI = "horizontal";//left or right
    private static string VERT = "vertical";//up or down

    private static string ATT = "attack";//punch or kick
    private static string MED_ATT = "medium_attack";//punch or kick
    private static string NAV = "nav";//enter or back
    private static string BLOCK = "block";

        //add listeners in the game scene manager based on what kind of listeners will be triggered by keyboard events
    //when keyboard input is triggered register an event
        // all listeners will be notified
	void Update () {
	    
	}

    public bool isStrafing() {
       return (isStrafingLeft() | isStrafingRight()) ;
    }
    public bool isStrafingLeft() {
        return (Input.GetAxis(HORI) == -1);
    }
    public bool isStrafingRight()
    {
        return (Input.GetAxis(HORI) == 1);
    }
    public bool isMovingVertically()
    {
        return (isJumping() | isCrouching());
    }
    public bool isJumping()
    {
        return (Input.GetAxis(VERT) == -1);
    }
    public bool isCrouching()
    {
        return (Input.GetAxis(VERT) == 1);
    }
}
