using UnityEngine;
using System.Collections;

public class BaseGun : Weapon {

	void Start () {
		LoadModel("Prefabs/Cube",new Vector3(0.5f,-0.25f,0.5f),Vector3.up,0.05f);
	}
	
	void OnFiring () {
		if(!enabled)return;
		GameObject bullet = Instantiate(Resources.Load("Prefabs/Bullet")) as GameObject;
		bullet.transform.position = cam.transform.position+cam.transform.forward*1+Vector3.down*0.5f;
		bullet.GetComponentInChildren<Bullet>().move = cam.transform.forward*10;
	}
}
