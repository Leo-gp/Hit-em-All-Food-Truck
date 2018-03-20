using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxObject : MonoBehaviour 
{
	public Vector2[] spawnPositions;
	public float speed;
	public float minInterval;
	public float maxInterval;

	private Rigidbody2D rb2d;

	void Awake ()
	{
		rb2d = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate ()
	{
		rb2d.velocity = new Vector2(-speed, 0);
	}
}