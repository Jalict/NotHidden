using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	private GameObject user;
	private bool paused = false;
	
	void Start () {
	}
	
	void Update () {
		// Keep locking the cursor
		if(user)Screen.lockCursor = !paused;
		
		if(Input.GetKeyDown(KeyCode.Escape) && user){
			paused = !paused;
			user.GetComponentInChildren<CharacterMotor>().SetControllable(!paused);
			(user.GetComponentsInChildren<MouseLook>()[0] as MonoBehaviour).enabled = !paused;
			(user.GetComponentsInChildren<MouseLook>()[1] as MonoBehaviour).enabled = !paused;
			user.GetComponentInChildren<WeaponHolder>().enabled = !paused;
			user.GetComponentInChildren<Character>().active = !paused;
		}
	}
	
	public void StartGame () {
		GameObject[] hunterSpawns = GameObject.FindGameObjectsWithTag("HunterSpawn");
		//GameObject[] soldierSpawns = GameObject.FindGameObjectsWithTag("SoldierSpawn");
		
		GameObject hunter = Instantiate(Resources.Load("Prefabs/Hunter")) as GameObject;
		hunter.transform.position = hunterSpawns[Random.Range(0,hunterSpawns.Length-1)].transform.position;
		user = hunter;
		
		GetComponent<Interface>().user = user.transform;
		GetComponent<Interface>().Init();
	}
	
	void OnGUI() {
		if(user)GUI.Box(new Rect(10,10,100,22),"Players: "+(Network.connections.Length+1).ToString());
	}
	
	void OnPlayerConnected(NetworkPlayer player){
		if(Network.isServer){
			networkView.RPC("LoadNetLevel",player,"test01");
		}
	}
	void OnLevelWasLoaded(int lvl) {
		StartGame();
	}
	[RPC]
	public void LoadNetLevel(string level) {
		Application.LoadLevel(level);
	}
}
