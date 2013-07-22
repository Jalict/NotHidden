using UnityEngine;
using System.Collections;

public class Grenade : Weapon {
	private float throwForce = 1000;

	void Start () {
		ammo = 3;
		maxAmmo = 3;
		removeOnEmpty = true;
		LoadModel("Prefabs/Models/pipebomb",new Vector3(0.5f,-0.25f,0.5f),Vector3.up,1f);
	}
	
	public override void OnFire(){
		if(!enabled)return;
		if(ammo>0){
			GameObject grenade = Network.Instantiate(Resources.Load("Prefabs/Grenade"),
				cam.transform.position+cam.transform.forward*1,
				Quaternion.identity, 0) as GameObject;
			grenade.rigidbody.AddForce(cam.transform.forward*throwForce);
			grenade.rigidbody.AddTorque(Vector3.right*500);
			ammo -= 1;
		}
	}
}
