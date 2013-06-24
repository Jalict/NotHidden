using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interface : MonoBehaviour {
	public Transform user;
	
	private List<Transform> jumpbar;
	private List<Transform> healthbar;
	private List<Transform> ammobar;
	private List<Transform> magbar;
	private Transform crosshair;
	
	private Hunter cha;
	private Health hea;
	private WeaponHolder wep;

	public void Init () {
		if(user)cha = user.GetComponentInChildren<Hunter>();
		if(user)hea = user.GetComponentInChildren<Health>();
		if(user)wep = user.GetComponentInChildren<WeaponHolder>();
		
		MakeCube(new Vector3(0,0,10),new Vector3(0.1f,0.1f,0.1f),Color.black);
		
		jumpbar = new List<Transform>();
		jumpbar.Add(MakeCube(new Vector3(-14,-9.6f,21),new Vector3(8.2f,1.2f,1),Color.black));
		jumpbar.Add(MakeCube(new Vector3(-14,-9.6f,20),new Vector3(8,1,1),(Color.cyan+Color.blue)/2)); // TODO convert unit to pixels
		
		healthbar = new List<Transform>();
		healthbar.Add(MakeCube(new Vector3(-14,-11,21),new Vector3(8.2f,1.2f,1),Color.black));
		healthbar.Add(MakeCube(new Vector3(-14,-11,20),new Vector3(8,1,1),(Color.red+Color.grey)/2));
		
		magbar = new List<Transform>();
		magbar.Add(MakeCube(new Vector3(14,-9.6f,21),new Vector3(8.2f,1.2f,1),Color.black));
		magbar.Add(MakeCube(new Vector3(14,-9.6f,20),new Vector3(8,1,1),(Color.grey)/2));
		
		ammobar = new List<Transform>();
		ammobar.Add(MakeCube(new Vector3(14,-11,21),new Vector3(8.2f,1.2f,1),Color.black));
		ammobar.Add(MakeCube(new Vector3(14,-11,20),new Vector3(8,1,1),Color.grey));
	}
	
	private Transform MakeCube (Vector3 pos, Vector3 scale, Color color) {
		Transform cube = (Instantiate(Resources.Load("Prefabs/Cube")) as GameObject).transform;
		cube.gameObject.layer = LayerMask.NameToLayer("GUI");
		cube.parent = transform;
		cube.localScale = scale;
		cube.localPosition = pos;
		cube.renderer.material.color = color;
		return cube;
	}
	
	void Update () {
		if(user){
			if(cha){
				jumpbar[1].localScale = new Vector3(8*(cha.energy/cha.maxEnergy),1,1);
				jumpbar[1].localPosition = new Vector3(-14-(4*(1-(cha.energy/cha.maxEnergy))),-9.6f,20); // TODO streamline this
			} else {
				jumpbar[1].localScale = Vector3.zero;
				jumpbar[0].localScale = Vector3.zero;
			}
			if(hea){
				healthbar[1].localScale = new Vector3(8*(hea.HP/hea.maxHP),1,1);
				healthbar[1].localPosition = new Vector3(-14-(4*(1-(hea.HP/hea.maxHP))),-11,20);
			} else {
				healthbar[1].localScale = Vector3.zero;
				healthbar[0].localScale = Vector3.zero;
			}
			if(wep){
				if(wep.maxAmmo>0){
					ammobar[1].localScale = new Vector3(8*(wep.ammo/wep.maxAmmo),1,1);
					ammobar[1].localPosition = new Vector3(14+(4*(1-(wep.ammo/wep.maxAmmo))),-11,20);
					ammobar[0].localScale = new Vector3(8.2f,1.2f,1);
				} else {
					ammobar[1].localScale = Vector3.zero;
					ammobar[0].localScale = Vector3.zero;
				}
				if(wep.maxMags>0){
					magbar[1].localScale = new Vector3(8*(wep.mags/wep.maxMags),1,1);
					magbar[1].localPosition = new Vector3(14+(4*(1-(wep.mags/wep.maxMags))),-9.6f,20);
					magbar[0].localScale = new Vector3(8.2f,1.2f,1);
				} else {
					magbar[1].localScale = Vector3.zero;
					magbar[0].localScale = Vector3.zero;
				}
			} else {
				ammobar[1].localScale = Vector3.zero;
				ammobar[0].localScale = Vector3.zero;
				magbar[1].localScale = Vector3.zero;
				magbar[0].localScale = Vector3.zero;
			}
		}
	}
}
