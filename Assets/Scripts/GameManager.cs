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
		
		bool[] hSpawns = new bool[GameObject.FindGameObjectsWithTag("HunterSpawn").Length];
		bool[] sSpawns = new bool[GameObject.FindGameObjectsWithTag("SoldierSpawn").Length];
		int hunter = Random.Range(0,Network.connections.Length);
		int spawn = -1;
		for(int i = 0; i < Network.connections.Length; i++){
			while(spawn<0){
				spawn = Random.Range(0,(i==hunter?hSpawns.Length:sSpawns.Length)-1);
				if(i==hunter?hSpawns[spawn]:sSpawns[spawn]){
					spawn = -1;
					if(i>=sSpawns.Length+(hunter<i?1:0) && i!=hunter){ // If there are no more soldier spawns
						spawn = Random.Range(0,sSpawns.Length-1);
					}
				} else {
					if(i==hunter)hSpawns[spawn] = true;
					else sSpawns[spawn] = true;
				}
			}
			networkView.RPC("SendGameStart", Network.connections[i], i==hunter, spawn);
		}
		SendGameStart(Network.connections.Length==hunter,0);
	}
	GameObject AddPlayerController (bool hunter, int spawn){ // Any Side
		GameObject[] hunterSpawns = GameObject.FindGameObjectsWithTag("HunterSpawn");
		GameObject[] soldierSpawns = GameObject.FindGameObjectsWithTag("SoldierSpawn");
		
		GameObject unit = Instantiate(Resources.Load("Prefabs/Hunter")) as GameObject;
		if(hunter) {
			unit.transform.position = hunterSpawns[spawn].transform.position;
		} else {
			unit.transform.position = soldierSpawns[spawn].transform.position;
		}
		unit.AddComponent<Controller>();
		unit.AddComponent<Health>();
		if(hunter) unit.AddComponent<Hunter>();
		
		WeaponHolder weapon = unit.AddComponent<WeaponHolder>();
		if(hunter){
			weapon.Give("Knife","Grenade");
		} else {
			weapon.Give("BaseGun");
		}
		
		unit.AddComponent<NetworkView>();
		unit.networkView.viewID = Network.AllocateViewID();
		unit.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		unit.networkView.observed = unit.transform;
		
		foreach(NetworkPlayer player in Network.connections){
			networkView.RPC("SendUserView", player, unit.networkView.viewID);
		}
		
		return unit;
	}
	
	void OnGUI() {
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
		//started = false;
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
	public void SendUserView(NetworkViewID view) { // Client Side
		GameObject unit = Instantiate(Resources.Load("Prefabs/Cube")) as GameObject;
		unit.AddComponent<NetworkView>();
		unit.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		unit.networkView.observed = unit.transform;
		unit.networkView.viewID = view;
	}
	[RPC]
	public void SendGameStart(bool hunter, int spawn) { // Client Side
		user = AddPlayerController(hunter, spawn);
		GetComponent<Interface>().user = user.transform;
		GetComponent<Interface>().Init();
		started = true;
	}
}
