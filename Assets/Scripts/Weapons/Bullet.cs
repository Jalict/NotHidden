using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public Vector3 move = Vector3.zero;
	internal float damage = 0;
	
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
	}
}
