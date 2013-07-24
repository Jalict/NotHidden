using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour {
	internal bool ending = false;
	internal string username;
	
	private GameObject user;
	private Hashtable playernames;
	private Hashtable playeralive;
	
	private bool typing = false;
	private bool paused = false;
	private bool started = false;
	
	// Chat Variables
	private List<string> chat = new List<string>();
	private string chatInput = "";
	private bool chatEnable = false; 
	private float chatDelay = 0;
	private float dChatDelay = 4;
	
	private float matchTime = 0;
	private float dMatchTime = 60;
	private NetworkPlayer currentHunter;
	
	void Start () {
		playernames = new Hashtable(Network.maxConnections);
		playeralive = new Hashtable(Network.maxConnections);
		name = "Manager";
	}
	
	void Update () {
		matchTime -= Time.deltaTime;
		chatDelay -= Time.deltaTime;
		
		if(Network.isServer && started){
			int numAlive = 0;
			foreach(bool a in playeralive.Values)
				if(a)numAlive++;
			if(matchTime<=0){
				networkView.RPC("SendMatchEnd",RPCMode.All,"TimeOut");
			}
			if(dMatchTime-matchTime > 5){
				if(!(bool)playeralive[currentHunter]){
					networkView.RPC("SendMatchEnd",RPCMode.All,"SoldierWin");
				}
				if(numAlive == 0){
					networkView.RPC("SendMatchEnd",RPCMode.All,"HunterWin");
				}
			}
		}
		
		if(Input.GetKeyDown(KeyCode.Return)) { 
			typing = !typing; 
			paused = typing; 
			if(typing) chatInput = "";
			else SendChat ();
		}
		if(!typing){
			if(started){
				if(Input.GetKeyDown(KeyCode.Escape)) paused = !paused;
			} else {
				paused = true;
			}	
		} else {
			paused = true;
			if(Input.GetKeyDown(KeyCode.Escape)) { typing = false; paused = false; }
		}
		Screen.lockCursor = !paused;
		if(user)user.GetComponentInChildren<Controller>().paused = paused;
		if(user)GetComponentInChildren<Interface>().paused = paused && !typing;
	}
	
	public void StartGame () { // Server Side
		if(!Network.isServer)return;
		
		// TODO improve spawn selection
		
		bool[] hSpawns = new bool[GameObject.FindGameObjectsWithTag("HunterSpawn").Length];
		bool[] sSpawns = new bool[GameObject.FindGameObjectsWithTag("SoldierSpawn").Length];
		int spawn = -1;
		
		NetworkPlayer[] players = new NetworkPlayer[playernames.Count];
		playernames.Keys.CopyTo(players,0);
		int hunter = Random.Range(0,players.Length);
		for(int i = 0; i < players.Length; i++){
			if(players[i]==Network.player){
				if(i==hunter)currentHunter=players[i];
				continue;
			}
			
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
			if(i==hunter)currentHunter=players[i];
			networkView.RPC("SendGameStart", players[i], i==hunter, spawn);
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
			weapon.Give("Knife","Grenade"); // TODO get loadout
		} else {
			weapon.Give("BaseGun");
		}
		
		unit.AddComponent<NetworkView>();
		unit.networkView.viewID = Network.AllocateViewID();
		unit.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		unit.networkView.observed = unit.transform;
		unit.AddComponent<Dummy>();
		
		networkView.RPC("SendUserView", RPCMode.OthersBuffered, unit.networkView.viewID);
		
		return unit;
	}
	
	void OnUserSpawn(NetworkPlayer player) { // Any Side
		if(!playeralive.ContainsKey(player))Debug.LogError("Player Not Found!");
		playeralive[player] = true;
	}
	void OnUserDeath(NetworkPlayer player) { // Any Side
		if(!playeralive.ContainsKey(player))Debug.LogError("Player Not Found!");
		playeralive[player] = false;
	}

	void SendChat() {
		if(chatInput.Length > 0) {
			networkView.RPC("OnChat",RPCMode.All, chatInput, Network.player);
			chatDelay = dChatDelay;
		}
		chatInput = "";
		typing = false;
		paused = false;
		chatEnable = false;
	}
	
	void OnGUI() {
		if(matchTime>0){
			GUILayout.Space(40);
			GUILayout.BeginHorizontal();
			int min = 0;
			int sec = (int)matchTime;
			while(sec>=60){
				min++;
				sec-=60;
			}
			float wid1 = 0;
			float wid2 = 0;
			GUI.skin.box.CalcMinMaxWidth(new GUIContent(min+":"+sec),out wid1,out wid2);
			GUILayout.Space(Screen.height/2-wid2);
			GUILayout.Box(min+":"+sec);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.BeginArea(new Rect(20,20,Screen.width/2,Screen.height/2));
		GUILayout.Box("Players: "+(playernames.Keys.Count).ToString(),GUILayout.Width(100));
		GUILayout.Space(5);
		if(!started){
			if(Network.isServer){
				if(GUILayout.Button("Start Game",GUILayout.Width(100))){
					StartGame();
				}
				GUILayout.Space(5);
			}
		}
		if(paused){
			//GUI.skin.box.alignment = TextAnchor.MiddleLeft;
			foreach(NetworkPlayer player in playernames.Keys){
				GUILayout.Box(playernames[player] + ((bool)playeralive[player]==true?" - Alive":""),GUILayout.Width(Screen.width/8));
				GUILayout.Space(5);
			}
			//GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		}
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(20,Screen.height/2,Screen.width/4,Screen.height/3));
		GUILayout.FlexibleSpace();
		if(chatDelay > 0 || typing || paused){
			GUI.skin.box.alignment = TextAnchor.LowerLeft;
			while(chat.Count>18)chat.RemoveAt(0);
			string showChat = chat.Count>0?chat[0]:"";
			for(int i = 1; i<Mathf.Max(18,chat.Count); i++){
				showChat = showChat + "\n" + (i<chat.Count?chat[i]:"");
			}
			GUILayout.Box(showChat);
			GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		}
		
		if(typing){
			GUI.SetNextControlName("ChatInput");
			chatInput = GUILayout.TextField(chatInput);
			GUI.FocusControl("ChatInput");
			if (Event.current.isKey && Event.current.keyCode == KeyCode.Return) {
				chatEnable = !chatEnable;
				if(!chatEnable)SendChat();
			}
		}
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(20,Screen.height/2+Screen.height/3,Screen.width/2,Screen.height/6));
		if((paused && !typing) || (!started)){
			GUILayout.Space(5);
			if(GUILayout.Button("Exit To Menu",GUILayout.Width(150)))Network.Disconnect();
			GUILayout.Space(5);
			if(GUILayout.Button("Exit To Desktop",GUILayout.Width(150)))Application.Quit();
		}
		GUILayout.EndArea();
	}
	void OnLevelWasLoaded(int lvl) { // Client Side
		networkView.RPC("OnPlayerJoin",RPCMode.OthersBuffered,username,Network.player);
		OnPlayerJoin(username, Network.player);
	}
	
	
	void OnPlayerConnected(NetworkPlayer player){ // Server Side
		if(Network.isServer){
			networkView.RPC("LoadNetLevel",player,"test01");
		}
	}
	void OnPlayerDisconnected(NetworkPlayer player){ // Server Side
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	void OnDisconnectedFromServer(NetworkDisconnection info) { // Client Side
		ending = true;
		Application.LoadLevel("MainMenu");
		Destroy(gameObject);
	}
	[RPC]
	public void OnChat(string input, NetworkPlayer player) {
		chat.Add(playernames[player]+": "+input);
		chatDelay = dChatDelay;
	}
	[RPC]
	public void OnPlayerJoin(string name, NetworkPlayer player) { // Client Side
		playernames.Add(player,name);
		playeralive.Add(player,false);
	}
	
	[RPC]
	public void LoadNetLevel(string level) { // Client Side
		Application.LoadLevel(level);
	}
	[RPC]
	public void SendUserView(NetworkViewID view) { // Any Side
		GameObject unit = new GameObject("Player");
		unit.AddComponent<NetworkView>();
		unit.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		unit.networkView.observed = unit.transform;
		unit.networkView.viewID = view;
		unit.AddComponent<Dummy>();
	}
	[RPC]
	public void SendGameStart(bool hunter, int spawn) { // Client Side
		user = AddPlayerController(hunter, spawn);
		GetComponent<Interface>().user = user.transform;
		GetComponent<Interface>().Init();
		started = true;
		paused = false;
		matchTime = dMatchTime;
	}
	[RPC]
	public void SendMatchEnd(string message) {
		matchTime = 0;
		Destroy(user);
		started = false;
		
		// TODO show winner
	}
}
