using UnityEngine;
using System.Collections;

public class GrenadeObj : MonoBehaviour {
	public float timer = 3;
	public float explosionRadius = 5;
	public float explosionForce = 50;
	public float damage = 60;

	void Start () {
	
	}
	
	void Update () {
		timer -= Time.deltaTime;
		if(timer <= 0){
			GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
			RaycastHit hit;
			foreach(GameObject unit in units){
				if(Physics.Raycast(transform.position,unit.transform.position-transform.position,out hit,explosionRadius)){
					if(hit.transform == unit.transform){
						unit.GetComponentInChildren<Health>().Damage(damage);
						if(unit.GetComponentInChildren<CharacterMotor>() != null){
							unit.GetComponentInChildren<CharacterMotor>().SetVelocity((unit.transform.position-transform.position).normalized*explosionForce);
						}
					}
				}
			}
			GameObject explosion = Instantiate(Resources.Load("Prefabs/Explosion")) as GameObject;
			explosion.transform.position = transform.position;
			Destroy(explosion,explosion.GetComponentInChildren<ParticleSystem>().duration);
			Destroy(gameObject);
		}
	}
}
