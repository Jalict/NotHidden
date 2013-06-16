using UnityEngine;
using System.Collections;

public class Knife : Weapon {
	private float hitDistance = 1.5f;
	private float hitForce = 2f;

	// Use this for initialization
	void Start () {
		LoadModel("Models/Weapons/Hunter/knife",new Vector3(0.25f,-0.25f,1),Quaternion.identity,5);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override void OnFire() {
		if(!enabled)return;
		RaycastHit hit;
		if(Physics.Raycast(cam.transform.position,cam.transform.forward,out hit,hitDistance)){
			if(hit.transform.GetComponent<Health>() != null){
				hit.transform.GetComponent<Health>().Damage(40);
			}
			if(hit.transform.GetComponent<CharacterMotor>() != null){
				hit.transform.GetComponent<CharacterMotor>().SetVelocity(cam.transform.forward*hitForce);
			}
		}
	}
}
