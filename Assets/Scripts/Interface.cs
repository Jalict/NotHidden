using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interface : MonoBehaviour {
	public Transform user;
	
	internal static float ratio;
	internal static Vector2 screensize;
	internal static Vector2 camsize;
	
	private Bar jumpbar;
	private Bar healthbar;
	private Bar ammobar;
	private Bar magbar;
	private Transform crosshair;
	
	private Material mat;
	private Hunter cha;
	private Health hea;
	private WeaponHolder wep;

	public void Init () {
		if(user)cha = user.GetComponentInChildren<Hunter>();
		if(user)hea = user.GetComponentInChildren<Health>();
		if(user)wep = user.GetComponentInChildren<WeaponHolder>();
		
		Camera cam = GetComponentInChildren<Camera>();
		ratio = cam.orthographicSize/cam.pixelHeight;
		camsize = new Vector2(cam.pixelWidth*ratio,cam.pixelHeight*ratio);
		screensize = camsize/ratio;
		
		MakeCube(new Vector3(0,0,10),new Vector3(0.1f,0.1f,0.1f),Color.black);
		
		jumpbar = new Bar(new Vector2(50,Screen.height-100),new Vector2(300,30),4,(Color.cyan+Color.blue)/2, Color.black,transform);
		healthbar = new Bar(new Vector2(50,Screen.height-50),new Vector2(300,30),4,(Color.red+Color.grey)/2, Color.black,transform);
		magbar = new Bar(new Vector2(Screen.width-300-50,Screen.height-100),new Vector2(300,30),4,Color.grey/2, Color.black,transform, true);
		ammobar = new Bar(new Vector2(Screen.width-300-50,Screen.height-50),new Vector2(300,30),4,Color.grey, Color.black,transform, true);
	}
	
	private Transform MakeCube (Vector3 pos, Vector3 scale, Color color) {
		Transform cube = (Instantiate(Resources.Load("Prefabs/Cube")) as GameObject).transform;
		cube.gameObject.layer = LayerMask.NameToLayer("GUI");
		cube.parent = transform;
		cube.localScale = scale;
		cube.localPosition = pos;
		cube.renderer.material = Resources.Load("Materials/blankui") as Material;
		cube.renderer.material.color = color;
		return cube;
	}
	
	void Update () {
		if(user){
			if(cha){
				jumpbar.update(cha.energy,cha.maxEnergy);
			}
			if(hea){
				healthbar.update(hea.HP,hea.maxHP);
			}
			if(wep){
				ammobar.update(wep.ammo,wep.maxAmmo,wep.maxAmmo>0);
				magbar.update(wep.mags,wep.maxMags,wep.maxMags>0);
			}
		}
	}

	private class Bar {
		private Transform borderbox;
		private Transform box;
		
		private Vector2 pos;
		private Vector2 size;
		//private float border;
		private bool invert;
		
		internal Bar(Vector2 pos, Vector2 size, float border, Color color, Color borderColor, Transform parent, bool invert = false){
			pos *= 2; size *= 2; border *= 2;
			this.pos = pos;
			//this.border = border;
			this.size = size;
			this.invert = invert;
			
			box = (Instantiate(Resources.Load("Prefabs/Cube")) as GameObject).transform;
			box.gameObject.layer = LayerMask.NameToLayer("GUI");
			box.parent = parent;
			box.localScale = new Vector3(size.x*Interface.ratio,size.y*Interface.ratio,1);
			box.localPosition = new Vector3((pos.x+size.x/2)*Interface.ratio - Interface.camsize.x,
				-(pos.y+size.y/2)*Interface.ratio + Interface.camsize.y,1);
			box.renderer.material = Resources.Load("Materials/blankui") as Material;
			box.renderer.material.color = color;
			
			borderbox = (Instantiate(Resources.Load("Prefabs/Cube")) as GameObject).transform;
			borderbox.gameObject.layer = LayerMask.NameToLayer("GUI");
			borderbox.parent = parent;
			borderbox.localScale = new Vector3((size.x+border*2)*Interface.ratio,(size.y+border*2)*Interface.ratio,1);
			borderbox.localPosition = new Vector3((pos.x+(size.x/2))*Interface.ratio - Interface.camsize.x,
				-(pos.y+(size.y/2))*Interface.ratio + Interface.camsize.y,2);
			borderbox.renderer.material = Resources.Load("Materials/blankui") as Material;
			borderbox.renderer.material.color = borderColor;
		}
		internal void update(float val, float max, bool enabled = true){
			if(enabled){
				float a = val/max;
				box.localScale = new Vector3(size.x*Interface.ratio*a,size.y*Interface.ratio,1);
				box.localPosition = new Vector3((pos.x+(size.x/2)*a*(invert?-1:1)+(invert?size.x:0))*Interface.ratio - Interface.camsize.x,
					-(pos.y+(size.y/2))*Interface.ratio + Interface.camsize.y,1);
			}
			box.renderer.enabled = enabled;
			borderbox.renderer.enabled = enabled;
		}
	}
}
