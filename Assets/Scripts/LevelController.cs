using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour 
{
	[System.Serializable] public class CustomSpawn
	{
		public Person.PersonType person;
		public int amount;
	}

	[Tooltip("Which persons can be spawned this level")]
	public Person[] persons;
	[Tooltip("Which bullets can be in this level")]
	public Bullet[] bullets;
	[Tooltip("How many persons will be spawned this level")]
	public int personsOnThisLevel;
	[Tooltip("Amount of times a person will be spawned in this level")]
	public CustomSpawn[] customSpawns;

	private int spawnedPersons;
}