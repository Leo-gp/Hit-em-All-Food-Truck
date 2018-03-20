using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour 
{
	[Header("References to attach")]
	public Person[] personsPrefabs;
	public Bullet[] bulletsPrefabs;
	public Image thirstIcon, hungerIcon;
	public GameObject happyIconPrefab, angryIconPrefab;
	public Text scoreTxt;
	public Text timerTxt;
	public GameObject gameOverScreen;
	public Text gameOverScoreTxt;
	public GameObject starIcon;
	public GameObject pauseScreen;
	[Header("Variables")]
	public float playerStartingSpeed;
	public Vector2[] spawnPoints;
	public float startingSpawnRate;
	public float maxSpawnRate;
	public float spawnRateIncreaseTime;
	public bool showAimLine;
	public float aimLineLength;
	public int scoreGainPerHappyPerson, scoreLossPerAngryPerson, scoreLossPerMiss;
	public float startingTime;
	public float timeGainPerHappyPerson;
	public float timeLossPerAngryPerson;
	public int[] scoresToEarnStars;
	public KeyCode alternativeThrowKey;
	public KeyCode switchAimDirectionKey;
	public KeyCode switchBulletKey;

	[HideInInspector] public float spawnRate;
	[HideInInspector] public float aimLineOffsetY = 1.5f;
	[HideInInspector] public bool gameIsOver;
	[HideInInspector] public int sequence;
	[HideInInspector] public int maxSequence;

	private bool personsSpawnerIsActive;
	private int score;
	private float UITextColorChangeDuration = 1f;
	private Color scoreTextDefaultColor;
	private Color timerTextDefaultColor;
	private float timer;
	private float elapsedTime;

	private PlayerController player;

	public static GameController instance;

	void Awake ()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);

		Time.timeScale = 1;
		player = FindObjectOfType<PlayerController>();
		if (player != null)
			player.moveSpeed = playerStartingSpeed;
		score = 0;
		scoreTxt.text = "Score: " + score;
		scoreTextDefaultColor = scoreTxt.color;
		timerTextDefaultColor = timerTxt.color;
		timer = startingTime * 60;
		gameIsOver = false;
		gameOverScreen.SetActive(false);
		spawnRate = startingSpawnRate;
		sequence = 0;
		maxSequence = 10;
		elapsedTime = 0f;
	}

	void Start ()
	{
		Pause();
	}

	void Update ()
	{
		elapsedTime += Time.deltaTime;

		if (spawnRate > maxSpawnRate)
			spawnRate -= elapsedTime / spawnRateIncreaseTime;

		player.moveSpeed = playerStartingSpeed + sequence / 2f;

		if (personsSpawnerIsActive == false)
		{
			StartCoroutine(PersonsSpawner());
		}
		
		HandleTimer();

		if (Input.GetKeyDown(KeyCode.P) && pauseScreen.activeInHierarchy == false)
		{
			Pause();
		}
	}

	Person GetRandomPerson ()
	{
		int n = Random.Range(0, personsPrefabs.Length);

		return personsPrefabs[n];
	}

	Vector2 GetRandomSpawnPoint ()
	{
		int n = Random.Range(0, spawnPoints.Length);

		return spawnPoints[n];
	}

	IEnumerator PersonsSpawner ()
	{
		personsSpawnerIsActive = true;

		while(personsSpawnerIsActive)
		{
			yield return new WaitForSeconds(Mathf.Clamp(spawnRate, 0.1f, 100f));

			InstantiateRandomPerson();
		}

		personsSpawnerIsActive = false;
	}

	void InstantiateRandomPerson ()
	{
		Person person = Instantiate(GetRandomPerson(), GetRandomSpawnPoint(), Quaternion.identity);

		if (person.transform.position.x > 0)
		{
			person.moveDirection = Person.MoveDirection.Left;
		}
		else
		{
			person.moveDirection = Person.MoveDirection.Right;
		}
	}

	public void Score (int value)
	{
		StartCoroutine(ScoreRoutine(value));
	}

	private IEnumerator ScoreRoutine (int value)
	{
		score += value;

		if (score < 0)
			score = 0;
		
		scoreTxt.text = "Score: " + score;

		if (value > 0)
			scoreTxt.color = Color.green;
		else if (value < 0)
			scoreTxt.color = Color.red;
		
		yield return new WaitForSeconds(UITextColorChangeDuration);

		scoreTxt.color = scoreTextDefaultColor;
	}

	void HandleTimer ()
	{
		if (timer > 1f)
			timer -= Time.deltaTime;
		else
		{
			GameOver();
		}

		string minutes = Mathf.Floor(timer / 60f).ToString("00");
		string seconds = Mathf.Floor(timer % 60f).ToString("00");

		timerTxt.text = "Timer: " + minutes + ":" + seconds;

		if (timer < 11f)
			timerTxt.color = Color.red;
	}

	void GameOver ()
	{
		if (gameIsOver == false)
		{
			gameIsOver = true;

			Time.timeScale = 0;

			gameOverScoreTxt.text = "Score: " + score;

			int earnedStars = 0;

			for (int i = 0; i < scoresToEarnStars.Length; i++)
			{
				if (score >= scoresToEarnStars[i])
				{
					earnedStars++;
				}
			}

			gameOverScreen.SetActive(true);

			GameObject starsPanel = gameOverScreen.GetComponentInChildren<GridLayoutGroup>().gameObject;

			for (int i = 0; i < earnedStars; i++)
			{
				Instantiate(starIcon, starsPanel.transform);
			}

			FreezeGame();

			AudioController.instance.Play("GameOver");
			AudioController.instance.Stop("ThrowEletricoSong");
		}
	}

	void FreezeGame ()
	{
		StopAllCoroutines(); // TODO Bug! Ao parar as rotinas, quando retomarem, estarão resetadas!

		player.enabled = false;
		this.enabled = false;

		Time.timeScale = 0;
	}

	void UnfreezeGame ()
	{
		personsSpawnerIsActive = false;

		player.enabled = true;
		this.enabled = true;

		Time.timeScale = 1;
	}

	public void GainTime (float amount)
	{
		StartCoroutine(GainTimeRoutine(amount));
	}

	public IEnumerator GainTimeRoutine (float amount)
	{
		timer += amount;
		timerTxt.color = Color.green;
		yield return new WaitForSeconds(UITextColorChangeDuration);
		timerTxt.color = timerTextDefaultColor;
	}

	public void LoseTime (float amount)
	{
		StartCoroutine(LoseTimeRoutine(amount));
	}

	public IEnumerator LoseTimeRoutine (float amount)
	{
		timer -= amount;
		timerTxt.color = Color.red;
		yield return new WaitForSeconds(UITextColorChangeDuration);
		timerTxt.color = timerTextDefaultColor;
	}

	public void Pause ()
	{
		FreezeGame();
		pauseScreen.SetActive(true);
	}

	public void Unpause ()
	{
		UnfreezeGame();
		pauseScreen.SetActive(false);
	}
}