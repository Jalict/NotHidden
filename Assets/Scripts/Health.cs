using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public float HP = 100;
	public float maxHP = 100;

	void Start () {
		gameObject.tag = "Unit";
	}
	
	public void Damage(float dmg){
		HP -= dmg;
		if(HP <= 0){
			Network.Destroy(gameObject);
		}
	}
}
