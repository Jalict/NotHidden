using UnityEngine;
using System.Collections;

public class Knife : Weapon {
	private float hitDistance = 1.5f;
	private float hitForce = 20f;

	void Start () {
		damage = 40;
		LoadModel("Prefabs/Models/knife",new Vector3(0.5f,-0.25f,0.5f),Vector3.forward,1);
	}
	
	public override void OnFire() {
		if(!enabled)return;
		RaycastHit hit;
		if(Physics.Raycast(cam.transform.position,cam.transform.forward,out hit,hitDistance,1<<LayerMask.NameToLayer("Enemies"))){
			if(hit.transform.GetComponentInChildren<Knife>() == transform)
				return;
			if(hit.transform.GetComponent<Health>() != null){
				hit.transform.GetComponent<Health>().Damage(damage);
			}
			if(hit.transform.GetComponent<CharacterMotor>() != null){
				hit.transform.GetComponent<CharacterMotor>().SetVelocity(cam.transform.forward*hitForce);
			}
		}
		GetComponent<NetworkView>().RPC("KnifeRaycast",RPCMode.Others,cam.transform.position,cam.transform.forward,hitDistance,damage);
	}
}
