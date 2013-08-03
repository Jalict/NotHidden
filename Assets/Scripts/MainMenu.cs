using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {
	public string[] levels = new string[0];
	
	private string username = "Rookie";
	
	private bool toggleServer = false;
	private string serverName = "Not A Hidden Server";
	private string serverPassword = "";
	private int serverMaxPlayers = 6;
	private bool serverVisible = true;
	private Vector2 levelScroll = Vector2.zero;
	private int levelSelect = 0;
	
	GameObject man;

	void Start () {
		if(Application.genuineCheckAvailable)
			if(!Application.genuine)
				Application.Quit();
		MasterServer.RequestHostList("NotHidden");
		if(PlayerPrefs.GetString("username").Length > 0)
			username = PlayerPrefs.GetString("username");
		if(PlayerPrefs.GetString("serverName").Length > 0)
			serverName = PlayerPrefs.GetString("serverName");
		serverPassword = PlayerPrefs.GetString("serverPassword");
		if(PlayerPrefs.GetInt("serverMaxPlayers") == 0)
			serverMaxPlayers = PlayerPrefs.GetInt("serverMaxPlayers");
		levelSelect = PlayerPrefs.GetInt("levelSelect");
	}
	
	void OnGUI () {
		float middleX = Screen.width/2;
		float middleY = Screen.height/2;
		
		GUI.Box(new Rect(20, 20, Screen.width/2-30,Screen.height-40),"");
		GUILayout.BeginArea(new Rect(20, 20, Screen.width/2-30,Screen.height-40));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Username: ");
		GUILayout.Space(5);
		username = GUILayout.TextField(username,16,GUILayout.Width(150));
		PlayerPrefs.SetString("username",username);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(5);
		// Loadout Button
		if(GUILayout.Button("Change Loadout", GUILayout.Width(120))){
			
		}
		GUILayout.Space(5);
		GUILayout.Space(10);
		
		// Server List
		if(GUILayout.Button("Refresh", GUILayout.Width(100))) MasterServer.RequestHostList("NotHidden");
		GUILayout.Space(5);
		HostData[] data = MasterServer.PollHostList();
		foreach (HostData host in data) {
			GUILayout.BeginHorizontal();
			string name = host.gameName + " " + host.connectedPlayers + "/" + host.playerLimit;
			GUILayout.Label(name);
			GUILayout.Space(5);
			
			string hostInfo = "[";
			foreach (string h in host.ip)
				hostInfo = hostInfo + h + ":" + host.port + " ";
			hostInfo = hostInfo + "]";
			GUILayout.Label(hostInfo);
			GUILayout.Space(5);
			
			GUILayout.Label(host.comment);
			GUILayout.Space(5);
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Connect")){
				Network.Connect(host);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Create Server"))
			toggleServer = !toggleServer;
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		// Top-Right Side
		Rect loadoutBox = new Rect(middleX+10, 20, Screen.width/2-30, (toggleServer)?(Screen.height/2-30):(Screen.height-40));
		GUI.Box(loadoutBox, "");
		
		// Bottom-Right Side, Start Server
		if(toggleServer){
			Rect hostBox = new Rect(middleX+10, middleY+10, middleX-30, middleY-30);
			Rect levelBox = new Rect(hostBox.x + hostBox.width/2, hostBox.y+10, hostBox.width/2, hostBox.height-10);
			int columns = Mathf.FloorToInt((levelBox.width-20)/100);
			int rows = Mathf.CeilToInt(levels.Length/columns);
			Rect viewBox = new Rect(0, 0, columns*100, rows*50);
			
			GUI.Box(hostBox, "");
			GUILayout.BeginArea(new Rect(hostBox.x+10,hostBox.y+10,hostBox.width/2-20,hostBox.height-20));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Server Name: ");
			GUILayout.FlexibleSpace();
			serverName = GUILayout.TextField(serverName,GUILayout.MinWidth(hostBox.width/4));
			PlayerPrefs.SetString("serverName",serverName);
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Server Password: ");
			GUILayout.FlexibleSpace();
			serverPassword = GUILayout.TextField(serverPassword,GUILayout.MinWidth(hostBox.width/4));
			PlayerPrefs.SetString("serverPassword",serverPassword);
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Max Players: ");
			GUILayout.FlexibleSpace();
			GUILayout.Label((serverMaxPlayers+1).ToString());
			GUILayout.Space(5);
			serverMaxPlayers = Mathf.RoundToInt(GUILayout.HorizontalSlider(serverMaxPlayers,1,9,GUILayout.MinWidth(hostBox.width/4)));
			PlayerPrefs.SetInt("serverMaxPlayers",serverMaxPlayers);
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			
			serverVisible = GUILayout.Toggle(serverVisible,"Visible on Server List");
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Create Server", GUILayout.Height(25), GUILayout.MinWidth(100))){
				//Network.incomingPassword = serverPassword;
				Network.InitializeServer(serverMaxPlayers,24024,!Network.HavePublicAddress());
				if(serverVisible)MasterServer.RegisterHost("NotHidden",serverName,"Description");
			}
			if(GUILayout.Button("Fuck Around", GUILayout.Height(25), GUILayout.MinWidth(100))){
				Application.LoadLevel(levels[levelSelect]);
				Destroy(gameObject);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
			levelScroll = GUI.BeginScrollView(levelBox,levelScroll,viewBox,false,false);
			levelSelect = GUI.SelectionGrid(viewBox,levelSelect,levels,columns);
			PlayerPrefs.SetInt("levelSelect",levelSelect);
			GUI.EndScrollView();
		}
	}
	void OnConnectedToServer() {
		man = Instantiate(Resources.Load("Prefabs/Interface")) as GameObject;
		man.GetComponent<GameManager>().username = username;
		DontDestroyOnLoad(man);
		Destroy(gameObject);
	}
	void OnServerInitialized() {
		man = Instantiate(Resources.Load("Prefabs/Interface")) as GameObject;
		man.GetComponent<GameManager>().username = username;
		DontDestroyOnLoad(man);
		Application.LoadLevel(levels[levelSelect]);
		Destroy(gameObject);
	}
}
