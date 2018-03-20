using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour 
{
	public enum TriggerType {Ground, Bound, ParallaxBound}

	public TriggerType triggerType;

	void OnTriggerEnter2D (Collider2D col)
	{
		if (triggerType == TriggerType.Ground)
		{
			if (col.GetComponent<Bullet>() != null)
			{
				Bullet bullet = col.GetComponent<Bullet>();

				bullet.FallBullet();
				col.enabled = false;
			}
		}

		if (triggerType == TriggerType.Bound)
		{
			if (col.GetComponent<Person>() != null)
			{
				col.GetComponent<Person>().Anger();
			}
		}

		if (triggerType == TriggerType.ParallaxBound)
		{
			if (col.GetComponent<ParallaxObject>() != null)
			{
				Destroy(col.gameObject);
			}
		}
	}
}