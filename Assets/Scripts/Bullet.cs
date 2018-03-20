using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
	public enum BulletType {Food, Drink}
	public enum ThrowDirection {Vertical, Diagonal}

	public BulletType bulletType;
	public ThrowDirection throwDirection;
	public float speed;
	public float rotationSpeed;

	[HideInInspector] public bool throwed;
	[HideInInspector] public bool stopMoving;

	private float diagonalForce;
	private float diagonalDirection;
	private int rendOrderInLayer;
	private bool rotatingLeft;
	private bool rotatingRight;

	private Rigidbody2D rb2d;
	private PlayerController player;
	private SpriteRenderer rend;

	void Awake ()
	{
		rb2d = GetComponent<Rigidbody2D>();
		player = FindObjectOfType<PlayerController>();
		rend = GetComponent<SpriteRenderer>();

		throwed = false;
		rb2d.gravityScale = 0;
		diagonalForce = 5f;
		rotatingLeft = false;
		rotatingRight = false;
	}

	void Update ()
	{
		if (stopMoving == true)
			return;

		if (throwed == false)
		{
			transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 1, player.transform.position.z);
		}
		else if (throwDirection == ThrowDirection.Diagonal)
		{
			rb2d.velocity = new Vector2(diagonalDirection * diagonalForce, rb2d.velocity.y);
			if (rotatingLeft)
				rb2d.rotation += rotationSpeed;
			else if (rotatingRight)
				rb2d.rotation -= rotationSpeed;
		}
		else
		{
			rb2d.rotation += rotationSpeed;
		}
	}

	public void ThrowBullet ()
	{
		throwed = true;
		rend.sortingOrder++;
		rb2d.gravityScale = Mathf.Clamp(speed, 0.1f, 100f);
		
		if (throwDirection == ThrowDirection.Diagonal)
		{
			SetDiagonalDirection();
		}

		AudioController.instance.Play("ThrowBullet");
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.GetComponent<Person>() != null)
		{
			Person person = col.GetComponent<Person>();

			HitPerson(person);
		}
	}

	void HitPerson (Person target)
	{
		if (target.leaving == true)
			return;
		
		if (target.desires.Contains(bulletType))
		{
			target.need --;

			foreach (BulletType desire in target.desires)
			{
				if (desire == bulletType)
				{
					Destroy(target.desiresGameObjects[target.desires.IndexOf(desire)]);
					target.desiresGameObjects.RemoveAt(target.desires.IndexOf(desire));
					target.desires.Remove(desire);
					break;
				}
			}

			if (target.personType == Person.PersonType.Drunk)
			{
				target.ChangeDirection();
			}

			AudioController.instance.Play("HitPersonDesire");
		}
		else
		{
			GameController.instance.Score(-GameController.instance.scoreLossPerMiss);
			AudioController.instance.Play("MissPersonDesire");
			if (GameController.instance.sequence > 0)
				GameController.instance.sequence--;
		}

		Destroy(gameObject);

		if (target.need <= 0)
		{
			target.Satisfy();
		}
	}

	void SetDiagonalDirection ()
	{
		if (player.aimingLeft)
		{
			diagonalDirection = -speed;
			rotatingLeft = true;
			rotatingRight = false;
		}
		else if (player.aimingRight)
		{
			diagonalDirection = speed;
			rotatingRight = true;
			rotatingLeft = false;
		}
	}

	public void FallBullet ()
	{
		stopMoving = true;
		rb2d.gravityScale = 0;
		rb2d.velocity = Vector2.zero;
		SpriteRenderer fallenBulletSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
		rend.sprite = fallenBulletSprite.sprite;
		AudioController.instance.Play("FallenBullet");
		GameController.instance.sequence = 0;
		GameController.instance.Score(-GameController.instance.scoreLossPerMiss);
		Destroy(gameObject, 2f);
	}
}