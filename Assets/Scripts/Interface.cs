using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interface : MonoBehaviour {
	public Transform user;
	
	private List<Transform> jumpbar;
	private List<Transform> healthbar;
	private Transform crosshair;
	
	private Character cha;
	private Health hea;

	void Start () {
		if(user)cha = user.GetComponentInChildren<Character>();
		if(user)hea = user.GetComponentInChildren<Health>();
		
		crosshair = MakeCube(new Vector3(0,0,10),new Vector3(0.1f,0.1f,0.1f),Color.black);
		
		jumpbar = new List<Transform>();
		jumpbar.Add(MakeCube(new Vector3(-11,-9.6f,21),new Vector3(8.2f,1.2f,1),Color.black));
		jumpbar.Add(MakeCube(new Vector3(-11,-9.6f,20),new Vector3(8,1,1),(Color.cyan+Color.blue)/2)); // TODO convert unit to pixels
		
		healthbar = new List<Transform>();
		healthbar.Add(MakeCube(new Vector3(-11,-11,21),new Vector3(8.2f,1.2f,1),Color.black));
		healthbar.Add(MakeCube(new Vector3(-11,-11,20),new Vector3(8,1,1),(Color.red+Color.grey)/2));
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
		if(cha){
			jumpbar[1].localScale = new Vector3(8*(cha.energy/cha.maxEnergy),1,1);
			jumpbar[1].localPosition = new Vector3(-11-(4*(1-(cha.energy/cha.maxEnergy))),-9.6f,20); // TODO streamline this
		}
		if(hea){
			healthbar[1].localScale = new Vector3(8*(hea.HP/hea.maxHP),1,1);
			healthbar[1].localPosition = new Vector3(-11-(4*(1-(hea.HP/hea.maxHP))),-11,20);
		}
	}
}
