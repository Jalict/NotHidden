using UnityEngine;
using System.Collections;

public class Dummy : MonoBehaviour {

	void Start () {
		if(GameObject.Find("Manager"))GameObject.Find("Manager").SendMessage("OnUserSpawn",networkView.owner);
	}
	
	void Update () {
	
	}
	
	void OnDestroy () {
		if(GameObject.Find("Manager"))GameObject.Find("Manager").SendMessage("OnUserDeath",networkView.owner);
	}
	
	[RPC]
	public void CreateDummyBullet (Vector3 pos, Vector3 dir, float damage) {
		GameObject bullet = Instantiate(Resources.Load("Prefabs/Effects/Bullet"),
			pos, Quaternion.identity) as GameObject;
		bullet.GetComponent<Bullet>().move = dir;
		bullet.GetComponent<Bullet>().damage = damage;
	}
	[RPC]
	public void KnifeRaycast (Vector3 pos, Vector3 dir, float dist, float damage) {
		RaycastHit hit;
		if(Physics.Raycast(pos,dir,out hit,dist,1<<LayerMask.NameToLayer("Enemies"))){
			if(hit.transform.GetComponent<Health>() != null){
				hit.transform.GetComponent<Health>().Damage(damage);
			}
			/*if(hit.transform.GetComponent<CharacterMotor>() != null){
				hit.transform.GetComponent<CharacterMotor>().SetVelocity(cam.transform.forward*hitForce);
			}*/
		}
	}
}
