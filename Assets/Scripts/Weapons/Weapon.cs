using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	protected GameObject model;
	protected Camera cam;
	internal int ammo = -1;
	internal int maxAmmo = -1;
	protected bool removeOnEmpty = false;
	protected float damage = 0;
	
	protected void LoadModel(string name, Vector3 pos, Vector3 rot, float scale) {
		cam = GetComponentInChildren<Camera>();
		model = Instantiate(Resources.Load(name)) as GameObject;
		model.transform.parent = cam.transform;
		model.transform.localPosition = pos;
		model.transform.localRotation = Quaternion.LookRotation(rot,Vector3.up);
		model.transform.localScale = Vector3.one * scale;
		model.layer = LayerMask.NameToLayer("Weapon");
	}
	
	public void Enable(bool enable){
		if(model != null) {
			model.renderer.enabled = enable;
		}
		enabled = enable;
	}
	
	void Update(){
		if(ammo==0 && removeOnEmpty){
			Destroy(model);
			Destroy(this);
		}
	}
	public virtual void OnFire(){
	}
	public virtual void OnFiring(){
	}
	public virtual void OnAltFire(){
	}
	public virtual void OnAltFiring(){
	}
	
	public bool empty {
		get { return ammo==0; }
	}
}
