using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public Vector3 move = Vector3.zero;
	
	void Start () {
	
	}
	
	void FixedUpdate () {
		RaycastHit hit;
		if (Physics.Raycast(transform.position,move,out hit,move.magnitude*Time.fixedDeltaTime)){
			Destroy(gameObject);
		}
		transform.position += move * Time.fixedDeltaTime;
	}
}
