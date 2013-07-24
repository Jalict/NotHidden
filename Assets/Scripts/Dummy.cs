using UnityEngine;
using System.Collections;

public class Dummy : MonoBehaviour {
	GameObject model;
	Animator anim;

	void Start () {
		if(GameObject.Find("Manager"))GameObject.Find("Manager").SendMessage("OnUserSpawn",networkView.owner);
		model = Instantiate(Resources.Load("Models/Alpha@t-pose_1")) as GameObject;
		model.transform.parent = transform;
		model.transform.localPosition = Vector3.down*0.9f;
		anim = model.GetComponent<Animator>();
		anim.runtimeAnimatorController = Resources.Load("Animations/AlphaController") as RuntimeAnimatorController;
		if(networkView.isMine)
			SetLayer(model.transform,"Self");
	}
	private void SetLayer(Transform t, string layer){
		for(int i = 0; i<t.childCount; i++){
			SetLayer(t.GetChild(i),layer);
		}
		t.gameObject.layer = LayerMask.NameToLayer(layer);
	}
	
	void Update () {
		anim.SetFloat("Speed",GetComponent<Controller>().moveInput.z);
		anim.SetFloat("Direction",GetComponent<Controller>().moveInput.x);
		model.transform.localPosition = Vector3.down*0.9f;
		model.transform.localRotation = Quaternion.identity;
	}
	
	void OnDestroy () {
		if(GameObject.Find("Manager"))GameObject.Find("Manager").SendMessage("OnUserDeath",networkView.owner);
		/*SetLayer(model.transform,"Default");
		model.transform.parent = transform.root;
		Destroy(model,60);*/
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
		if(Physics.Raycast(pos,dir,out hit,dist,(1<<LayerMask.NameToLayer("Enemies")) | (1<<LayerMask.NameToLayer("Self")))){
			if(hit.transform.GetComponent<Health>() != null){
				hit.transform.GetComponent<Health>().Damage(damage);
			}
			/*if(hit.transform.GetComponent<CharacterMotor>() != null){
				hit.transform.GetComponent<CharacterMotor>().SetVelocity(cam.transform.forward*hitForce);
			}*/
		}
	}
}
