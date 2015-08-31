using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour {

    public float fadeSpeed = 1.5f;
    private bool sceneStarting;
    private bool fadingActive;
    private float threshold = 0.25f;
    private RawImage background;

    void Awake()
    {
        background = gameObject.GetComponent<RawImage>();
    }

    void Start()
    {
        sceneStarting = true;
        fadingActive = true;
    }

	// Update is called once per frame
	void Update () {
        if (sceneStarting)
        {
            StartScene();
        }
	}

    void FadeToClear()
    {
        Debug.Log("Fading to clear");
        fadingActive = true;
        // Lerp the colour of the texture between itself and transparent.

        Color temp = GetComponent<GUITexture>().color;
        temp.a = temp.a - (fadeSpeed * Time.deltaTime);
        GetComponent<GUITexture>().color = temp;
        GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.clear, fadeSpeed * Time.deltaTime);
    }

    void FadeToBlack()
    {
        fadingActive = true;
        // Lerp the colour of the texture between itself and black.
        GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.black, fadeSpeed * Time.deltaTime);
    }

    void StartScene()
    {

        // Fade the texture to clear.
        FadeToClear();

        // If the texture is almost clear...
        if (GetComponent<GUITexture>().color.a <= threshold)
        {
            // ... set the colour to clear and disable the GUITexture.
            GetComponent<GUITexture>().color = Color.clear;
            GetComponent<GUITexture>().enabled = false;

            // The scene is no longer starting.
            sceneStarting = false;
            fadingActive = false;
        }
    }

    public void EndScene()
    {
        // Make sure the texture is enabled.
        GetComponent<GUITexture>().enabled = true;

        // Start fading towards black.
        FadeToBlack();

        // If the screen is almost black...
        if (GetComponent<GUITexture>().color.a >= 1 - threshold)
            fadingActive = false;
    }

    public bool isFading()
    {
        return fadingActive;
    }

}
