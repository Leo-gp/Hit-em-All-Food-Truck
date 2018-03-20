using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour 
{
	public static SceneController instance;

	void Awake ()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	public void RestartScene ()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void GoToScene (string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void GoToNextScene (float delayTime)
	{
		StartCoroutine(GoToNextSceneRoutine(delayTime));
	}

	public IEnumerator GoToNextSceneRoutine (float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void QuitGame ()
	{
		Application.Quit();
	}
}