using UnityEngine;
using System.Collections;

public class GrenadeObj : MonoBehaviour {
	public float timer = 3;

	void Start () {
	
	}
	
	void Update () {
		timer -= Time.deltaTime;
		if(timer <= 0){
			Destroy(gameObject);
		}
	}
}
