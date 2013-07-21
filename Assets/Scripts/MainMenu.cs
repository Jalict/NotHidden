using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	private string ports = "24024";
	private int port = 24024;
	private string maxps = "16";
	private int maxp = 16;
	private string password = "";
	private string ip = "0.0.0.0";
	
	GameObject man;

	void Start () {
		if(Application.genuineCheckAvailable)
			if(!Application.genuine)
				Application.Quit();
	}
	
	void OnGUI () {
		// Start Server
		maxps = GUI.TextField(new Rect(130,20,22,20),maxps,2);
		int.TryParse(maxps,out maxp);
		maxps = maxp>0?maxp.ToString():"";
		
		ports = GUI.TextField(new Rect(155,20,42,20),ports,5);
		int.TryParse(ports,out port);
		ports = port>0?port.ToString():"";
		
		password = GUI.TextField(new Rect(300,20,168,20),password,16);
		
		if(GUI.Button(new Rect(20,15,100,30),"Start Server")){
			if(port > 0){
				Network.incomingPassword = password;
				Network.InitializeServer(maxp,port,false); // nat = !Network.HavePublicAddress()
			}
		}
		
		
		// Join Server
		ports = GUI.TextField(new Rect(237,70,42,20),ports,5);
		int.TryParse(ports,out port);
		ports = port>0?port.ToString():"";
		
		ip = GUI.TextField(new Rect(130,70,104,20),ip,15);
		password = GUI.TextField(new Rect(300,70,168,20),password,16);
		
		if(GUI.Button(new Rect(20,65,100,30),"Join Server")){
			if(port > 0 && ip.Length > 7){
				Network.Connect(ip,port,password);
			}
		}
	}
	void OnConnectedToServer() {
		man = Instantiate(Resources.Load("Prefabs/Interface")) as GameObject;
		DontDestroyOnLoad(man);
		Destroy(gameObject);
	}
	void OnServerInitialized() {
		Network.logLevel = NetworkLogLevel.Informational;
		man = Instantiate(Resources.Load("Prefabs/Interface")) as GameObject;
		DontDestroyOnLoad(man);
		Application.LoadLevel("test01");
		Destroy(gameObject);
	}
}
