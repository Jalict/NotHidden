using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponHolder : MonoBehaviour {
	
	// Weapon variables
	public bool inputFire = false;
	public bool inputFiring = false;
	public bool inputAltFire = false;
	public bool inputAltFiring = false;
	public float inputScroll = 0;
	
	private List<Weapon> weapons;
	private int currentWeapon;
	private bool ready = false;
	private KeyCode[] numbers = {KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4,KeyCode.Alpha5,
								KeyCode.Alpha6,KeyCode.Alpha7,KeyCode.Alpha8,KeyCode.Alpha9,KeyCode.Alpha0};
	
	
	public void Give(params string[] weaponNames) {
		// Check if inited
		if(!ready){
			weapons = new List<Weapon>();
			currentWeapon = 0;
			ready = true;
		}
		
		// Setup weapons
		for (int i = 0; i < weaponNames.Length; i++) {
			weapons.Add(gameObject.AddComponent(weaponNames[i]) as Weapon);
			weapons[weapons.Count-1].Enable(false);
		}
		weapons[currentWeapon].Enable(true);
	}
	
	// Update is called once per frame
	void Update () {
		if(!ready)return;
		if(weapons.Count < 1)return;
		
		// Check if current weapon is gone
		if(!weapons[currentWeapon]){
			currentWeapon += 1;
			while(currentWeapon >= weapons.Count)currentWeapon-=weapons.Count;
			while(currentWeapon < 0)currentWeapon+=weapons.Count;
			weapons[currentWeapon].Enable(true);
		}
		
		// Switch weapon on number keys
		for(int i = 0; i<numbers.Length; i++){
			if(Input.GetKeyDown(numbers[i])){
				if(weapons.Count > i && weapons[i] != null){
					if(weapons[currentWeapon])weapons[currentWeapon].Enable(false);
					currentWeapon = i;
					weapons[currentWeapon].Enable(true);
				}
			}
		}
		
		// Scroll weapons
		if(inputScroll != 0 && weapons.Count > 0){
			if(weapons[currentWeapon])weapons[currentWeapon].Enable(false);
			currentWeapon += (int)Mathf.Sign(inputScroll);
			
			while(currentWeapon >= weapons.Count)currentWeapon-=weapons.Count;
			while(currentWeapon < 0)currentWeapon+=weapons.Count;
			while(!weapons[currentWeapon]){
				currentWeapon += (int)Mathf.Sign(inputScroll);
				while(currentWeapon >= weapons.Count)currentWeapon-=weapons.Count;
				while(currentWeapon < 0)currentWeapon+=weapons.Count;
			}
			
			weapons[currentWeapon].Enable(true);
		}
		
		// Fire gadget
		if(inputFire){
			SendMessage("OnFire");
		}
		if(inputFiring){
			SendMessage("OnFiring");
		}
		if(inputAltFire){
			SendMessage("OnAltFire");
		}
		if(inputAltFiring){
			SendMessage("OnAltFiring");
		}
	}
	public float maxAmmo{
		get { return ready?(weapons[currentWeapon] != null)?weapons[currentWeapon].maxAmmo:0:0; }
	}
	public float ammo{
		get { return ready?(weapons[currentWeapon] != null)?weapons[currentWeapon].ammo:0:0; }
	}
	public float maxMags{
		get { return ready?(weapons[currentWeapon] is BaseGun)?(weapons[currentWeapon] as BaseGun).maxMags:0:0; }
	}
	public float mags{
		get { return ready?(weapons[currentWeapon] is BaseGun)?(weapons[currentWeapon] as BaseGun).mags:0:0; }
	}
}
