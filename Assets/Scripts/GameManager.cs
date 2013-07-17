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
	private MemoryStream stream;
	private BinaryFormatter format;
	
	void Start () {
		players = new Hashtable(Network.maxConnections);
		
		inputNames = new List<string>();
		inputData = new List<float>();
		stream = new MemoryStream();
		format = new BinaryFormatter();
	}
	
	void Update () {
		if(started){
			Screen.lockCursor = !paused;
			if(Network.isServer){ // Server Side
				if(!paused){
					UpdateInput(ref user.GetComponent<Controller>().input);
				} else {
					ResetInput(ref user.GetComponent<Controller>().input);
				}
			} else { // Client Side
				// Get Input Data
				CheckInput();
				
				// Merge Input Data
				/*string[] data = new string[inputNames.Count+inputData.Count];
				inputNames.CopyTo(data,0);
				inputData.CopyTo(data,inputNames.Count);
				Debug.LogError(data[inputNames.Count]);*/
				
				// Convert To Byte Array
				format.Serialize(stream,inputNames.ToArray());
				format.Serialize(stream,inputData.ToArray());
				
				// Send Data
				networkView.RPC("GetUserInput", RPCMode.Server, Network.player, stream.ToArray()); // TODO maybe make player watchers instead
				
				// Clear Input Data
				stream.Flush();
				inputNames.Clear();
				inputData.Clear();
			}
			
			// Pause Game On Escape
			if(Input.GetKeyDown(KeyCode.Escape)){
				paused = !paused;
				if(user.GetComponentInChildren<CharacterMotor>())
					user.GetComponentInChildren<CharacterMotor>().SetControllable(!paused);
				if(user.GetComponentInChildren<WeaponHolder>())
					user.GetComponentInChildren<WeaponHolder>().enabled = !paused;
				if(user.GetComponentInChildren<Hunter>())
					user.GetComponentInChildren<Hunter>().active = !paused;
			}
		}
	}
	
	public void StartGame () { // Server Side
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
	GameObject AddPlayerController (GameObject[] hunterSpawns, GameObject[] soldierSpawns, NetworkPlayer p){ // Server Side
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
			networkView.RPC("SendUserView", player, unit.networkView.viewID, p.Equals(player));
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
	
	private void UpdateInput(ref PlayerInput input){ // Local Server Side
		input.look.x = Input.GetAxis("Mouse X");
		input.look.y = Input.GetAxis("Mouse Y");
		input.move.x = Input.GetAxis("Horizontal");
		input.move.y = Input.GetAxis("Vertical");
	}
	private void ResetInput(ref PlayerInput input){ // Local Server Side
		input.move = Vector2.zero;
		input.look = Vector2.zero;
	}
	private void CheckInput(){ // Client Side
		AddInput("lookX",Input.GetAxis("Mouse X"));
		AddInput("lookY",Input.GetAxis("Mouse Y"));
		AddInput("moveX",Input.GetAxis("Horizontal"));
		AddInput("moveY",Input.GetAxis("Vertical"));
	}
	private void AddInput(string name, float data){ // Client Side
		inputNames.Add(name);
		inputData.Add(data);
	}
	
	void OnPlayerConnected(NetworkPlayer player){ // Server Side
		if(Network.isServer){
			foreach(DictionaryEntry pl in players){
				(pl.Value as GameObject).networkView.SetScope(player,false);
			}
			networkView.RPC("LoadNetLevel",player,"test01");
		}
	}
	
	[RPC]
	public void LoadNetLevel(string level) { // Client Side
		Application.LoadLevel(level);
	}
	[RPC]
	public void UserLevelLoaded(NetworkPlayer player) { // Server Side
		foreach(DictionaryEntry pl in players){
			(pl.Value as GameObject).networkView.SetScope(player,true);
		}
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
		GetComponent<Interface>().user = user.transform;
		GetComponent<Interface>().Init();
		started = true;
	}
	[RPC]
	public void GetUserInput(NetworkPlayer player, byte[] data) { // Server Side
		stream.Write(data, 0, data.Length);
		stream.Position=0;
		string[] names = (string[]) format.Deserialize(stream);
		float[] input = (float[]) format.Deserialize(stream);
		stream.Flush();
		
		if(players.Contains(player)){
			Controller controller = (players[player] as GameObject).GetComponent<Controller>();
			for(int i = 0; i < input.Length; i++){
				switch(names[i]){
				case "lookX":
					controller.input.look.x = input[i];
					break;
				case "lookY":
					controller.input.look.y = input[i];
					break;
				case "moveX":
					controller.input.move.x = input[i];
					break;
				case "moveY":
					controller.input.move.y = input[i];
					break;
				}
			}
		}
	}
}
