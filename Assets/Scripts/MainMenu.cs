using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	private string ports = "24024";
	private int port = 24024;
	private string maxps = "16";
	private int maxp = 16;
	private string servername = "Server Name";
	private string password = "";
	private string ip = "0.0.0.0";
	private string username = "Player";
	
	GameObject man;

	void Start () {
		if(Application.genuineCheckAvailable)
			if(!Application.genuine)
				Application.Quit();
		MasterServer.RequestHostList("NotHidden");
	}
	
	void OnGUI () {
		// Start Server
		/*maxps = GUI.TextField(new Rect(130,20,22,20),maxps,2);
		int.TryParse(maxps,out maxp);
		maxps = maxp>0?maxp.ToString():"";
		
		ports = GUI.TextField(new Rect(155,20,42,20),ports,5);
		int.TryParse(ports,out port);
		ports = port>0?port.ToString():"";
		
		password = GUI.TextField(new Rect(300,20,168,20),password,16);*/
		
		GUILayout.BeginArea(new Rect(20, 20, Screen.width/2,Screen.height-60));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Username: ");
		GUILayout.Space(5);
		username = GUILayout.TextField(username,16,GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(5);
		// Loadout Button
		if(GUILayout.Button("Change Loadout", GUILayout.Width(120))){
			
		}
		GUILayout.Space(5);
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Start Server", GUILayout.Width(100))){
			if(port > 0){
				Network.incomingPassword = password;
				Network.InitializeServer(maxp-1,port,!Network.HavePublicAddress()); // nat = !Network.HavePublicAddress()
				MasterServer.RegisterHost("NotHidden",servername,"Description");
			}
		}
		GUILayout.Space(5);
		maxps = GUILayout.TextField(maxps,2, GUILayout.Width(22));
		int.TryParse(maxps,out maxp);
		maxps = maxp>0?maxp.ToString():"";
		GUILayout.Space(5);
		/*ports = GUILayout.TextField(ports,5, GUILayout.Width(100));
		int.TryParse(ports,out port);
		ports = port>0?port.ToString():"";
		GUILayout.Space(5);*/
		servername = GUILayout.TextField(servername,32, GUILayout.Width(320));
		//GUILayout.Space(5);
		//password = GUILayout.TextField(password,16, GUILayout.Width(160));
		GUILayout.EndHorizontal();
		GUILayout.Space(10);
		
		
		// Join Server
		/*ports = GUI.TextField(new Rect(237,70,42,20),ports,5);
		int.TryParse(ports,out port);
		ports = port>0?port.ToString():"";
		
		ip = GUI.TextField(new Rect(130,70,104,20),ip,15);
		password = GUI.TextField(new Rect(300,70,168,20),password,16);
		
		if(GUI.Button(new Rect(20,65,100,30),"Join Server")){
			if(port > 0 && ip.Length > 7){
				Network.Connect(ip,port,password);
			}
		}*/
		
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
		
		GUILayout.EndArea();
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
		Application.LoadLevel("test01");
		Destroy(gameObject);
	}
}
