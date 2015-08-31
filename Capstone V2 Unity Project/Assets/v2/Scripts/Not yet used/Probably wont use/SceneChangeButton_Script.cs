using UnityEngine;
using System.Collections;

//Activates the scene change when user clicks on object
public class SceneChangeButton_Script : MonoBehaviour
{
		public GameObject sceneManager;
        public GameObject screenFader;
		public GameSceneState nextScene;

        private bool sceneChangeStarted = false;
	
		void Start ()
		{
            if (!sceneManager)
                Debug.Log("SceneChangeButton_Script: Set Scene manager reference");
            if (!screenFader)
                Debug.Log("SceneChangeButton_Script: Set screenFader reference");
        }

        public void Update()
        {
            //if the scene is done fading out change the levels
            if (sceneChangeStarted)
            {
                screenFader.GetComponent<ScreenFader>().EndScene();
                if (!screenFader.GetComponent<ScreenFader>().isFading())
                    SwitchLevels();
                else
                    Debug.Log("Can't switch scenes due to scene fade");
            }
        }

		public void OnMouseDown ()
		{
            Debug.Log("Scene Change Button Clicked");
            sceneChangeStarted = true;
		}

		public void SwitchLevels ()
		{
			PlayerPrefs.SetString ("currScene", nextScene.ToString ());
            sceneManager.GetComponent<GameSceneLoader>().changeToScreen(nextScene);
			Application.LoadLevel (nextScene.ToString ());
		}
}