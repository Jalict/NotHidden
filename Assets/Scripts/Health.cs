using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public float HP = 100;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Damage(float dmg){
		HP -= dmg;
		if(HP <= 0){
			Destroy(gameObject);
		}
	}
}
