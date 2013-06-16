using UnityEngine;
using System.Collections;

public class GrenadeObj : MonoBehaviour {
	public float timer = 3;

	void Start () {
	
	}
	
	void Update () {
		timer -= Time.deltaTime;
		if(timer <= 0){
			GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
			foreach(GameObject unit in units){
				// TODO raytrace
			}
			GameObject explosion = Instantiate(Resources.Load("Prefabs/Explosion")) as GameObject;
			explosion.transform.position = transform.position;
			Destroy(gameObject);
		}
	}
}
