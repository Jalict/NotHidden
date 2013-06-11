using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	public GameObject model;
	
	// Use this for initialization
	void Start () {
		
	}
	public void LoadModel(string name, Vector3 pos, Quaternion rot, float scale) {
		model = Instantiate(Resources.Load(name)) as GameObject;
		model.transform.parent = GetComponentInChildren<Camera>().transform;
		model.transform.localPosition = pos;
		model.transform.localRotation = Quaternion.identity;
		model.transform.localScale = Vector3.one * scale;
		model.layer = LayerMask.NameToLayer("Weapon");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Enable(bool enable){
		if(model != null) {
			model.renderer.enabled = enable;
		}
		enabled = enable;
	}
}
