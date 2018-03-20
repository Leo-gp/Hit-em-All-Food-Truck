using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxHolder : MonoBehaviour 
{
	public ParallaxObject parallaxObject;
	public Transform scenario;

	private bool parallaxIsActive;

	void Start ()
	{
		parallaxIsActive = true;
		StartCoroutine(InstantiateObjects());
	}

	IEnumerator InstantiateObjects ()
	{
		while(parallaxIsActive == true)
		{
			int n = Random.Range(0, parallaxObject.spawnPositions.Length);
			Instantiate(parallaxObject, parallaxObject.spawnPositions[n], Quaternion.identity, scenario);
			float r = Random.Range(parallaxObject.minInterval, parallaxObject.maxInterval);
			yield return new WaitForSeconds(r);
		}
	}
}