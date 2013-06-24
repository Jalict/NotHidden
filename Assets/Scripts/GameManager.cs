using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	private GameObject user;
	private List<GameObject> players;
	private bool paused = false;
	private bool started = false;
	
	void Start () {
		players = new List<GameObject>();
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
			if(user.GetComponentInChildren<Hunter>())user.GetComponentInChildren<Hunter>().active = !paused;
		}
		if(Network.isClient){
			
		}
	}
	
	public void StartGame () {
		if(Network.isServer || true){
			GameObject[] hunterSpawns = GameObject.FindGameObjectsWithTag("HunterSpawn");
			//GameObject[] soldierSpawns = GameObject.FindGameObjectsWithTag("SoldierSpawn");
			
			GameObject hunter = Instantiate(Resources.Load("Prefabs/Hunter")) as GameObject;
			hunter.transform.position = hunterSpawns[Random.Range(0,hunterSpawns.Length-1)].transform.position;
			if(Network.isServer){
				hunter.AddComponent<NetworkView>();
				hunter.networkView.viewID = Network.AllocateViewID();
				hunter.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
				hunter.networkView.observed = hunter.transform;
				players.Add(hunter);
			}
			user = hunter;
		}
		GetComponent<Interface>().user = user.transform;
		GetComponent<Interface>().Init();
		started = true;
	}
	
	void OnGUI() {
		if(user)GUI.Box(new Rect(10,10,100,22),"Players: "+(Network.connections.Length+1).ToString());
	}
	
	void OnPlayerConnected(NetworkPlayer player){
		if(Network.isServer){
			for(int i = 0; i<players.Count; i++){
				players[i].networkView.SetScope(player,false);
			}
			networkView.RPC("LoadNetLevel",player,"test01");
		}
	}
	void OnLevelWasLoaded(int lvl) {
		//if(Network.isServer)
			StartGame();
		if(Network.isClient)networkView.RPC("UserLevelLoaded",RPCMode.Server,Network.player);
	}
	[RPC]
	public void LoadNetLevel(string level) {
		Application.LoadLevel(level);
	}
	[RPC]
	public void UserLevelLoaded(NetworkPlayer pl) {
		for(int i = 0; i<players.Count; i++){
			players[i].networkView.SetScope(pl,true);
			networkView.RPC("LevelStart", pl, players[i].networkView.viewID);
		}
	}
	[RPC]
	public void LevelStart(NetworkViewID data) {
		GameObject cube = Instantiate(Resources.Load("Prefabs/Cube")) as GameObject;
		cube.AddComponent<NetworkView>();
		cube.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		cube.networkView.observed = cube.transform;
		cube.networkView.viewID = data;
	}
}
