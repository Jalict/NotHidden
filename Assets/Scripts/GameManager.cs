using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	private GameObject user;
	private bool paused = false;
	
	void Start () {
		StartGame();
	}
	
	void Update () {
		// Keep locking the cursor
		if(!paused)Screen.lockCursor = true;
		
		if(Input.GetKeyDown(KeyCode.Escape)){
			paused = !paused;
			user.GetComponentInChildren<CharacterMotor>().SetControllable(!paused);
			(user.GetComponentsInChildren<MouseLook>()[0] as MonoBehaviour).enabled = !paused;
			(user.GetComponentsInChildren<MouseLook>()[1] as MonoBehaviour).enabled = !paused;
			user.GetComponentInChildren<WeaponHolder>().enabled = !paused;
			user.GetComponentInChildren<Character>().active = !paused;
		}
	}
	
	void StartGame () {
		GameObject[] hunterSpawns = GameObject.FindGameObjectsWithTag("HunterSpawn");
		GameObject[] soldierSpawns = GameObject.FindGameObjectsWithTag("SoldierSpawn");
		
		GameObject hunter = Instantiate(Resources.Load("Prefabs/Hunter")) as GameObject;
		hunter.transform.position = hunterSpawns[Random.Range(0,hunterSpawns.Length-1)].transform.position;
		user = hunter;
		
		GetComponent<Interface>().user = user.transform;
		GetComponent<Interface>().Init();
	}
}
