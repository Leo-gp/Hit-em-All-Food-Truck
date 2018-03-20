using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour 
{
	[HideInInspector] public float moveSpeed;
	[HideInInspector] public bool aimingLeft;
	[HideInInspector] public bool aimingRight;

	private List<Bullet> bullets;
	private List<Image> bulletsListImages;
	private int maxBulletsCount;
	private Bullet selectedBullet;

	private Rigidbody2D rb2d;
	private LineRenderer lineRend;

	void Awake ()
	{
		rb2d = GetComponent<Rigidbody2D>();
		lineRend = GetComponentInChildren<LineRenderer>();
		bullets = new List<Bullet>();
		bulletsListImages = new List<Image>();
		aimingLeft = true;
		aimingRight = false;
		maxBulletsCount = 3;
		selectedBullet = null;
	}

	void Start ()
	{
		GameObject bulletsListPanel = GetComponentInChildren<GridLayoutGroup>().gameObject;

		for (int i = 0; i < bulletsListPanel.transform.childCount; i++)
		{
			bulletsListImages.Add(bulletsListPanel.transform.GetChild(i).GetComponent<Image>());
		}

		UpdateBullets();
	}

	void Update ()
	{
		HandleThrows();

		HandleBullets();

		HandleAim();
	}

	void FixedUpdate ()
	{
		HandleMovement();
	}

	void HandleMovement ()
	{
		if (Input.GetButton("Horizontal"))
		{
			float moveH = Input.GetAxis("Horizontal");

			rb2d.velocity = new Vector2(moveH * moveSpeed, 0);
		}
	}

	Bullet GetRandomBullet ()
	{
		if (GameController.instance.bulletsPrefabs.Length == 0)
		{
			Debug.LogError("BulletsPrefabs list is empty!");
			return null;
		}

		int n = Random.Range(0, GameController.instance.bulletsPrefabs.Length);

		return GameController.instance.bulletsPrefabs[n];
	}

	void PickUpNewBullet ()
	{
		if (selectedBullet != null)
		{
			Destroy(selectedBullet.gameObject);
		}
		selectedBullet = Instantiate(bullets[0]);
		selectedBullet.transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
	}

	void AddNewBulletToList ()
	{
		if (bullets.Count < maxBulletsCount)
		{
			bullets.Add(GetRandomBullet());
		}
	}

	void HandleThrows ()
	{
		if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(GameController.instance.alternativeThrowKey))
		{
			if (bullets.Count > 0)
			{
				selectedBullet.ThrowBullet();
				UpdateBullets();
			}
		}
	}

	void UpdateBullets ()
	{
		if (bullets.Count > 0)
		{
			bullets.Remove(bullets[0]);
		}

		selectedBullet = null;

		while (bullets.Count < maxBulletsCount)
		{
			AddNewBulletToList();
		}

		UpdateBulletsListImages();

		PickUpNewBullet();
	}

	void UpdateBulletsListImages()
	{
		if (bullets.Count >= bulletsListImages.Count)
		{
			for (int i = 0; i < bulletsListImages.Count; i++)
			{
				bulletsListImages[i].sprite = bullets[i].GetComponent<SpriteRenderer>().sprite;
			}
		}
		else
		{
			Debug.LogError("bulletsListImages must have the same size or less than bullets variable");
		}
	}

	void HandleAim ()
	{
		if (Input.GetKeyDown(GameController.instance.switchAimDirectionKey))
		{
			if (aimingLeft == true)
			{
				aimingLeft = false;
				aimingRight = true;
			}
			else if (aimingRight == true)
			{
				aimingRight = false;
				aimingLeft = true;
			}
			else
			{
				aimingLeft = true;
			}
		}

		DrawAimLine();
	}

	void DrawAimLine ()
	{
		if (GameController.instance.showAimLine == true)
		{
			if (lineRend != null)
			{
				if (lineRend.enabled == false)
					lineRend.enabled = true;

				lineRend.SetPosition(0, new Vector3(transform.position.x, 
													transform.position.y - GameController.instance.aimLineOffsetY, 
													transform.position.z));

				if (selectedBullet.throwDirection == Bullet.ThrowDirection.Vertical)
				{
					lineRend.SetPosition(1, new Vector3(transform.position.x,
						transform.position.y - GameController.instance.aimLineLength - GameController.instance.aimLineOffsetY, 
														transform.position.z));
				}
				else if (aimingLeft == true)
				{
					lineRend.SetPosition(1, new Vector3(transform.position.x - GameController.instance.aimLineLength / 2, 
						transform.position.y - GameController.instance.aimLineLength / 2 - GameController.instance.aimLineOffsetY, 
														transform.position.z));
				}
				else if (aimingRight == true)
				{
					lineRend.SetPosition(1, new Vector3(transform.position.x + GameController.instance.aimLineLength / 2, 
						transform.position.y - GameController.instance.aimLineLength / 2 - GameController.instance.aimLineOffsetY,
														transform.position.z));
				}
			}
			else
			{
				Debug.LogError("Player LineRender has not been found!");
			}
		}
		else
		{
			if (lineRend != null)
				lineRend.enabled = false;
		}
	}

	void HandleBullets ()
	{
		if (Input.GetKeyDown(GameController.instance.switchBulletKey))
		{
			if (selectedBullet != null)
			{
				Destroy(selectedBullet.gameObject);
				AudioController.instance.Play("SwitchBullet");
			}
			
			UpdateBullets();
		}
	}
}