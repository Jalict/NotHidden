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
				// Get Input Data
				CheckInput();
				
				// Merge Input Data
				string[] data = new string[inputNames.Count+inputData.Count];
				inputNames.CopyTo(data,0);
				inputData.ConvertAll<string>(FloatToString).CopyTo(data,inputNames.Count);
				
				// Convert To Byte Array
				MemoryStream stream = new MemoryStream();
				BinaryFormatter format = new BinaryFormatter();
				format.Serialize(stream,data);
				
				// Send Data
				networkView.RPC("GetUserInput", RPCMode.Server, Network.player, stream.ToArray()); // TODO make player watchers instead
				
				// Clear Input Data
				inputNames.Clear();
				inputData.Clear();
			}
			
			// Pause Game On Escape
			if(Input.GetKeyDown(KeyCode.Escape)){
				paused = !paused;
				user.GetComponentInChildren<CharacterMotor>().SetControllable(!paused);
				user.GetComponentInChildren<WeaponHolder>().enabled = !paused;
				if(user.GetComponentInChildren<Hunter>())user.GetComponentInChildren<Hunter>().active = !paused;
			}
		}
	}
	private string FloatToString(float a){
		return a.ToString();
	}
	
	public void StartGame () {
		if(!Network.isServer)return;
		
		GameObject[] hunterSpawns = GameObject.FindGameObjectsWithTag("HunterSpawn");
		GameObject[] soldierSpawns = GameObject.FindGameObjectsWithTag("SoldierSpawn");
		
		foreach(NetworkPlayer p in Network.connections){
			AddPlayerController (hunterSpawns, soldierSpawns, p);
		}
		user = AddPlayerController(hunterSpawns, soldierSpawns, Network.player);
		
		foreach(NetworkPlayer player in Network.connections){
			networkView.RPC("SendGameStart", player);
		}
		
		GetComponent<Interface>().user = user.transform;
		GetComponent<Interface>().Init();
		started = true;
	}
	GameObject AddPlayerController (GameObject[] hunterSpawns, GameObject[] soldierSpawns, NetworkPlayer p){
		GameObject unit = Instantiate(Resources.Load("Prefabs/Hunter")) as GameObject;
		unit.transform.position = hunterSpawns[Random.Range(0,hunterSpawns.Length-1)].transform.position;
		unit.AddComponent<Controller>();
		unit.AddComponent<Health>();
		
		unit.AddComponent<NetworkView>();
		unit.networkView.viewID = Network.AllocateViewID();
		unit.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		unit.networkView.observed = unit.transform;
		players.Add(p, unit);
		
		foreach(NetworkPlayer player in Network.connections){
			networkView.RPC("SendUserView", player, p.Equals(player));
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
	void OnLevelWasLoaded(int lvl) {
		if(Network.isClient)networkView.RPC("UserLevelLoaded",RPCMode.Server,Network.player);
	}
	
	private void UpdateInput(ref PlayerInput input){
		input.look.x = Input.GetAxis("Mouse X");
		input.look.y = Input.GetAxis("Mouse Y");
		input.move.x = Input.GetAxis("Horizontal");
		input.move.y = Input.GetAxis("Vertical");
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
	
	void OnPlayerConnected(NetworkPlayer player){
		if(Network.isServer){
			foreach(DictionaryEntry pl in players){
				(pl.Value as GameObject).networkView.SetScope(player,false);
			}
			networkView.RPC("LoadNetLevel",player,"test01");
		}
	}
	
	[RPC]
	public void LoadNetLevel(string level) {
		Application.LoadLevel(level);
	}
	[RPC]
	public void UserLevelLoaded(NetworkPlayer player) {
		foreach(DictionaryEntry pl in players){
			(pl.Value as GameObject).networkView.SetScope(player,true);
		}
		//networkView.RPC("SendUserView", player, (pl.Value as GameObject).networkView.viewID);
	}
	[RPC]
	public void SendUserView(NetworkViewID data, bool local) {
		GameObject cube = Instantiate(Resources.Load("Prefabs/Cube")) as GameObject;
		cube.AddComponent<NetworkView>();
		cube.networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		cube.networkView.observed = cube.transform;
		cube.networkView.viewID = data;
		
		if(local)user = cube;
	}
	[RPC]
	public void SendGameStart() {
		GetComponent<Interface>().user = user.transform;
		GetComponent<Interface>().Init();
	}
	[RPC]
	public void GetUserInput(NetworkPlayer player, params string[] data) {
		if(players.Contains(player)){
			Controller controller = (players[player] as GameObject).GetComponent<Controller>();
			for(int i = 0; i < data.Length/2; i++){
				switch(data[i]){
				case "lookX":
					controller.input.look.x = float.Parse(data[i+data.Length/2]);
					break;
				case "lookY":
					controller.input.look.y = float.Parse(data[i+data.Length/2]);
					break;
				case "moveX":
					controller.input.move.x = float.Parse(data[i+data.Length/2]);
					break;
				case "moveY":
					controller.input.move.y = float.Parse(data[i+data.Length/2]);
					break;
				}
			}
		}
	}
}
