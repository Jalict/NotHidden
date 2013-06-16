using UnityEngine;
using System.Collections;

public class Grenade : Weapon {
	private float throwForce = 1000;

	void Start () {
		ammo = 3;
		LoadModel("Models/Weapons/Hunter/grenade",new Vector3(0.5f,-0.25f,0.5f),Vector3.up,0.1f);
	}
	
	public override void OnFire(){
		if(!enabled)return;
		GameObject grenade = Instantiate(Resources.Load("Prefabs/Grenade")) as GameObject;
		grenade.transform.position = cam.transform.position+cam.transform.forward*1;
		grenade.rigidbody.AddForce(cam.transform.forward*throwForce);
		grenade.rigidbody.AddTorque(Vector3.right*500);
	}
}
