using UnityEngine;
using System.Collections;

public class BaseGun : Weapon {
	public float spread = 0.01f;
	public float fireRate = 30f;
	public float muzzleSpeed = 100f;
	public int bulletsPerShot = 1;
	public float recoil = 1f;
	private float timer;
	private float fireTime;

	void Start () {
		LoadModel("Prefabs/Cube",new Vector3(0.5f,-0.25f,0.5f),Vector3.up,0.05f);
		
		timer = 0;
		fireTime = 1/fireRate;
	}
	
	void OnFiring () {
		if(!enabled)return;
		if(timer<=0){
			GameObject bullet = Instantiate(Resources.Load("Prefabs/Bullet")) as GameObject;
			bullet.transform.position = cam.transform.position+cam.transform.right*0.5f-cam.transform.up*0.25f;
			
			Vector3 dir = cam.transform.forward;
			dir = Vector3.RotateTowards(dir,Random.value<0.5?cam.transform.right:-cam.transform.right,Random.value*spread,0);
			dir = Vector3.RotateTowards(dir,Random.value<0.5?cam.transform.up:-cam.transform.up,Random.value*spread,0);
			bullet.GetComponentInChildren<Bullet>().move = dir*muzzleSpeed;
			bullet.GetComponentInChildren<Bullet>().damage = 10;
			
			cam.transform.Rotate(recoil,0,0);
			timer += fireTime;
		} else {
			timer -= Time.deltaTime;
		}
	}
}
