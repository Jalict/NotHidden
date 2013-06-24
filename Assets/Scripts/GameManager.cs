using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	private GameObject user;
	private Hashtable players;
	private bool paused = false;
	private bool started = false;
	
	private List<string> inputNames;
	private List<float> inputData;
	
	void Start () {
		players = new Hashtable(Network.maxConnections);
		inputNames = new List<string>();
		inputData = new List<float>();
	}
	
	void Update () {
		if(started){
			Screen.lockCursor = !paused;
			if(Network.isServer){
				if(!paused){
					UpdateInput(ref user.GetComponent<Controller>().input);
				} else {
					ResetInput(ref user.GetComponent<Controller>().input);
				}
			} else if(Network.isClient){
				CheckInput();
				networkView.RPC("GetUserInput",RPCMode.Server,inputNames.ToArray(),inputData.ToArray());
			}
		}
		if(Input.GetKeyDown(KeyCode.Escape) && started){
			paused = !paused;
			user.GetComponentInChildren<CharacterMotor>().SetControllable(!paused);
			user.GetComponentInChildren<WeaponHolder>().enabled = !paused;
			if(user.GetComponentInChildren<Hunter>())user.GetComponentInChildren<Hunter>().active = !paused;
		}
	}
	
	public void StartGame () {
		if(Network.isServer || true){
			GameObject[] hunterSpawns = GameObject.FindGameObjectsWithTag("HunterSpawn");
			//GameObject[] soldierSpawns = GameObject.FindGameObjectsWithTag("SoldierSpawn");
			
			GameObject hunter = Instantiate(Resources.Load("Prefabs/Hunter")) as GameObject;
			hunter.transform.position = hunterSpawns[Random.Range(0,hunterSpawns.Length-1)].transform.position;
			hunter.AddComponent<Controller>();
			hunter.AddComponent<Health>();
			if(Network.isServer){
				hunter.AddComponent<NetworkView>();
				hunter.networkView.viewID = Network.AllocateViewID();
				hunter.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
				hunter.networkView.observed = hunter.transform;
				players.Add(Network.player.ipAddress,hunter);
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
			foreach(DictionaryEntry pl in players){
				(pl.Value as GameObject).networkView.SetScope(player,false);
			}
			networkView.RPC("LoadNetLevel",player,"test01");
		}
	}
	void OnLevelWasLoaded(int lvl) {
		StartGame();
		if(Network.isClient)networkView.RPC("UserLevelLoaded",RPCMode.Server,Network.player);
	}
	
	private void UpdateInput(ref PlayerInput input){
		input.look.x = Input.GetAxis("Mouse X");
		input.look.y = Input.GetAxis("Mouse Y");
	}
	private void ResetInput(ref PlayerInput input){
		input.move = Vector2.zero;
		input.look = Vector2.zero;
	}
	private void CheckInput(){
		AddInput("lookX",Input.GetAxis("Mouse X"));
		AddInput("lookY",Input.GetAxis("Mouse Y"));
		AddInput("moveX",Input.GetAxis("Horizontal"));
		AddInput("moveY",Input.GetAxis("Vertical"));
	}
	private void AddInput(string name, float data){
		inputNames.Add(name);
		inputData.Add(data);
	}
	
	[RPC]
	public void LoadNetLevel(string level) {
		Application.LoadLevel(level);
	}
	[RPC]
	public void UserLevelLoaded(NetworkPlayer player) {
		foreach(DictionaryEntry pl in players){
			(pl.Value as GameObject).networkView.SetScope(player,true);
			networkView.RPC("LevelStart", player, (pl.Value as GameObject).networkView.viewID);
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
	[RPC]
	public void GetUserInput(NetworkPlayer player, string[] names, float[] data) {
		if(players.Contains(player.ipAddress)){
			Controller controller = (players[player.ipAddress] as GameObject).GetComponent<Controller>();
			for(int i = 0; i < names.Length; i++){
				switch(names[i]){
				case "lookX":
					controller.input.look.x = data[i];
					break;
				case "lookY":
					controller.input.look.y = data[i];
					break;
				case "moveX":
					controller.input.move.x = data[i];
					break;
				case "moveY":
					controller.input.move.y = data[i];
					break;
				}
			}
		}
	}
}
