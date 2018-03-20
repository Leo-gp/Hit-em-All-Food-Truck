using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Person : MonoBehaviour
{
	public enum PersonType {Normal, Fat, Small, Drunk}
	public enum MoveDirection {Left, Right}

	public PersonType personType;
	public float moveSpeed;
	public int startingNeed;

	[HideInInspector] public MoveDirection moveDirection;
	[HideInInspector] public int need;
	[HideInInspector] public List<Bullet.BulletType> desires;
	[HideInInspector] public List<GameObject> desiresGameObjects;
	[HideInInspector] public bool leaving;

	private Vector2 iconOffsetHappy = new Vector2(0f, 1.25f);
	private Vector2 iconOffsetAngry = new Vector2(2.15f, 1.25f);
	private float iconLifetime = 2f;

	private Rigidbody2D rb2d;
	private GameObject desiresCanvas;

	void Awake ()
	{
		rb2d = GetComponent<Rigidbody2D>();
		desiresCanvas = GetComponentInChildren<Canvas>().gameObject;
		desires = new List<Bullet.BulletType>();
		desiresGameObjects = new List<GameObject>();
	}

	void Start ()
	{
		need = startingNeed;

		for (int i = 0; i < need; i++)
		{
			AddRandomDesire();
		}
	}

	void FixedUpdate ()
	{
		Move();
	}

	void Move ()
	{
		if (leaving == false)
		{
			if (moveDirection == MoveDirection.Left)
			{
				rb2d.velocity = new Vector2(-moveSpeed, 0);
			}
			else
			{
				rb2d.velocity = new Vector2(moveSpeed, 0);
			}
		}
		else
		{
			rb2d.velocity = new Vector2(0, -moveSpeed);
		}
	}

	public void Satisfy ()
	{
		leaving = true;
		InstantiateIcon(GameController.instance.happyIconPrefab, iconOffsetHappy, true);
		Destroy(gameObject, 3f);
		GameController.instance.Score(GameController.instance.scoreGainPerHappyPerson);
		AudioController.instance.Play("HappyPerson");
		GameController.instance.GainTime(GameController.instance.timeGainPerHappyPerson);
		if (GameController.instance.sequence < GameController.instance.maxSequence)
			GameController.instance.sequence++;
	}

	public void Anger ()
	{
		GameObject icon = InstantiateIcon(GameController.instance.angryIconPrefab, iconOffsetAngry, false);
		Destroy(gameObject);
		Destroy(icon, iconLifetime);
		GameController.instance.Score(-GameController.instance.scoreLossPerAngryPerson);
		AudioController.instance.Play("AngryPerson");
		GameController.instance.LoseTime(GameController.instance.timeLossPerAngryPerson);
		GameController.instance.sequence = 0;
	}

	public void ChangeDirection ()
	{
		if (moveDirection == MoveDirection.Left)
		{
			moveDirection = MoveDirection.Right;
		}
		else
		{
			moveDirection = MoveDirection.Left;
		}
	}

	Bullet.BulletType GetRandomDesire ()
	{
		int n = Random.Range(0, 2);

		return (n == 0 ? Bullet.BulletType.Drink : Bullet.BulletType.Food);
	}

	void AddRandomDesire ()
	{
		Bullet.BulletType desire = GetRandomDesire();

		desires.Add(desire);

		if (desire == Bullet.BulletType.Drink)
		{
			desiresGameObjects.Add(Instantiate(GameController.instance.thirstIcon, desiresCanvas.transform).gameObject);
		}
		else
		{
			desiresGameObjects.Add(Instantiate(GameController.instance.hungerIcon, desiresCanvas.transform).gameObject);
		}
	}

	public GameObject InstantiateIcon (GameObject icon, Vector2 offset, bool makeIconChild)
	{
		GameObject iconObj = Instantiate(icon);
		float offsetX = offset.x;

		if (makeIconChild)
			iconObj.transform.SetParent(this.transform);

		if (moveDirection == MoveDirection.Right)
			offsetX = -offsetX;
			
		iconObj.transform.position = new Vector3(transform.position.x + offsetX, 
												 transform.position.y + offset.y, 
												 transform.position.z);
		return iconObj;
	}
}