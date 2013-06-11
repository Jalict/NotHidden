using UnityEngine;
using System.Collections;

public class Knife : Weapon {

	// Use this for initialization
	void Start () {
		LoadModel("Models/Weapons/Hunter/knife",new Vector3(0.25f,-0.25f,1),Quaternion.identity,5);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnFire() {
		model.transform.Translate(Vector3.forward);
	}
}
