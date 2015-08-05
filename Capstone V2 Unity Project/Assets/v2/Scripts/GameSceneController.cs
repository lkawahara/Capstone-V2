using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

public enum GameSceneState
{//different scenes that can be switched to 
		//scenes must be titled to match the casing and spelling of enum
		Title,
		GameModeSelect,
		CharacterSelect,
		Fight
}
public class GameSceneController : MonoBehaviour
{
		//Controller to be used to only switch between screens
		private GameSceneState[] toScenes; //names of scenes this current scene can access
		//private GameSceneState nextDefaultScreenName; //name of scene to auto go to on timeout
         //level indexes defined in build settings
        private static Dictionary<int, GameSceneState[]> sceneMap = new Dictionary<int, GameSceneState[]>() 
            { 
                    { 0, new GameSceneState[] {  //TITLE
                        GameSceneState.GameModeSelect
                    } },
                    { 1, new GameSceneState[] {   //GAME MODE SELECT
                        GameSceneState.CharacterSelect //same screen multiple options
                    } },
                    { 2, new GameSceneState[] { //CHARACTER SELECT
                        GameSceneState.Fight
                    } },
                    { 3, new GameSceneState[] { //FIGHT
                        GameSceneState.CharacterSelect, //reselect characters
                        GameSceneState.GameModeSelect, //change game mode
                        GameSceneState.Fight //rematch with same fighters
                    } }
            };
   
		public void Start ()
		{
			sceneMap.TryGetValue (Application.loadedLevel, out toScenes);
		}

		public void changeToNextScreen ()
		{
            string nextLevel = toScenes[0].ToString();
            Debug.Log("Changing to scene: " + nextLevel);
			Application.LoadLevel (nextLevel);
		}

        public void changeToScreen(int i) {
            Application.LoadLevel(toScenes[i].ToString());
        }

        public void changeToScreen(GameSceneState nextState)
        {
            Boolean notFound = true;
            foreach(GameSceneState scene in toScenes){
                if(scene == nextState){
                    Application.LoadLevel(scene.ToString());
                    notFound = false;
                }
            }
            if (notFound) {
                Debug.Log("Cannot change to that screen");
            }
        }
}
