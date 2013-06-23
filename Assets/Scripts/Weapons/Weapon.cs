using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	protected GameObject model;
	protected Camera cam;
	protected int ammo = -1;
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
	
	public virtual void OnFire(){
	}
	public virtual void OnFiring(){
	}
}
