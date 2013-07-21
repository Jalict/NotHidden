using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour {
	private GameObject user;
	private Hashtable players;
	private bool paused = false;
	private bool started = false;
	
	void Start () {
		players = new Hashtable(Network.maxConnections);
	}
	
	void Update () {
		if(started){
			if(Input.GetKeyDown(KeyCode.Escape)) paused = !paused;
			
			Screen.lockCursor = !paused;
			user.GetComponentInChildren<Controller>().paused = paused;
		}
	}
	
	public void StartGame () { // Server Side
		if(!Network.isServer)return;
		
		networkView.RPC("SendGameStart", RPCMode.Others);
		
		user = AddPlayerController();
		GetComponent<Interface>().user = user.transform;
		GetComponent<Interface>().Init();
		started = true;
	}
	GameObject AddPlayerController (){ // Server Side
		GameObject[] hunterSpawns = GameObject.FindGameObjectsWithTag("HunterSpawn");
		GameObject[] soldierSpawns = GameObject.FindGameObjectsWithTag("SoldierSpawn");
		
		GameObject unit = Instantiate(Resources.Load("Prefabs/Hunter")) as GameObject;
		unit.transform.position = hunterSpawns[Random.Range(0,hunterSpawns.Length-1)].transform.position;
		unit.AddComponent<Controller>();
		unit.AddComponent<Health>();
		WeaponHolder weapon = unit.AddComponent<WeaponHolder>();
		weapon.Load("BaseGun","Grenade","Knife");
		
		unit.AddComponent<NetworkView>();
		unit.networkView.viewID = Network.AllocateViewID();
		unit.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		unit.networkView.observed = unit.transform;
		//players.Add(p, unit);
		
		foreach(NetworkPlayer player in Network.connections){
			networkView.RPC("SendUserView", player, unit.networkView.viewID, false);
		}
		
		return unit;
	}
	
	void OnGUI() {
		GUI.Box(new Rect(10,10,100,22),"Players: "+(Network.connections.Length+1).ToString());
		if(Network.isServer && !started){
			if(GUI.Button(new Rect(10,36,100,22),"Start Game")){
				StartGame();
			}
		}
	}
	void OnLevelWasLoaded(int lvl) { // Client Side
		if(Network.isClient)networkView.RPC("UserLevelLoaded",RPCMode.Server,Network.player);
	}
	
	void OnPlayerConnected(NetworkPlayer player){ // Server Side
		if(Network.isServer){
			foreach(DictionaryEntry pl in players){
				(pl.Value as GameObject).networkView.SetScope(player,false);
			}
			networkView.RPC("LoadNetLevel",player,"test01");
		}
	}
	void OnPlayerDisconnected(NetworkPlayer player){ // Server Side
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	void OnDisconnectedFromServer(NetworkDisconnection info) { // Client Side
		started = false;
	}
	
	[RPC]
	public void LoadNetLevel(string level) { // Client Side
		Application.LoadLevel(level);
	}
	[RPC]
	public void UserLevelLoaded(NetworkPlayer player) { // Server Side
		/*foreach(DictionaryEntry pl in players){
			(pl.Value as GameObject).networkView.SetScope(player,true);
		}*/
		//networkView.RPC("SendUserView", player, (pl.Value as GameObject).networkView.viewID);
	}
	[RPC]
	public void SendUserView(NetworkViewID data, bool local) { // Client Side
		GameObject cube = Instantiate(Resources.Load("Prefabs/Cube")) as GameObject;
		cube.AddComponent<NetworkView>();
		cube.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		cube.networkView.observed = cube.transform;
		cube.networkView.viewID = data;
		
		if(local)user = cube;
	}
	[RPC]
	public void SendGameStart() { // Client Side
		user = AddPlayerController();
		GetComponent<Interface>().user = user.transform;
		GetComponent<Interface>().Init();
		started = true;
	}
}
