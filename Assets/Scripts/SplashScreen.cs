using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour 
{
	public VideoPlayer splashScreenVideo;
	public Text splashScreenText;

	void Start ()
	{
		splashScreenText.enabled = false;
		Invoke("ShowSplashScreenText", 4f);
		SceneController.instance.GoToNextScene((float)splashScreenVideo.clip.length);
	}

	void ShowSplashScreenText ()
	{
		splashScreenText.enabled = true;
	}
}