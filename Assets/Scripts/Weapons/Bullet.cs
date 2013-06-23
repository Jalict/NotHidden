using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	internal Vector3 move = Vector3.zero;
	internal float damage = 0;
	internal float range = 100; 
	
	void Start () {
	
	}
	
	void Update () {
		RaycastHit hit;
		if (Physics.Raycast(transform.position,move,out hit,move.magnitude*Time.deltaTime)){
			if(hit.transform.GetComponentInChildren<Health>()){
				hit.transform.GetComponentInChildren<Health>().Damage(damage);
			}
			Destroy(gameObject);
		}
		transform.position += move * Time.deltaTime;
		range -= move.magnitude * Time.deltaTime;
		if(range<=0){
			Destroy(gameObject);
		}
	}
}
