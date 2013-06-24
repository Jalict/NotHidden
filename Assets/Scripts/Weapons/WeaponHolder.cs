using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponHolder : MonoBehaviour {
	
	// Weapon variables
	public string fireKey = "Fire1";
	public string scrollAxis = "Mouse ScrollWheel";
	public List<string> weaponNames;
	private List<Weapon> weapons;
	private int currentWeapon;
	private KeyCode[] numbers = {KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4,KeyCode.Alpha5,
								KeyCode.Alpha6,KeyCode.Alpha7,KeyCode.Alpha8,KeyCode.Alpha9,KeyCode.Alpha0};

	void Start () {
		// Setup weapons
		weapons = new List<Weapon>();
		int i = weaponNames.Count;
		while(i-- > 0){
			weapons.Add(gameObject.AddComponent(weaponNames[i]) as Weapon);
			weapons[weapons.Count-1].Enable(false);
		}
		currentWeapon = 0;
		weapons[currentWeapon].Enable(true);
	}
	
	// Update is called once per frame
	void Update () {
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
		if(Input.GetAxis(scrollAxis) != 0 && weapons.Count > 0){
			if(weapons[currentWeapon])weapons[currentWeapon].Enable(false);
			currentWeapon += (int)Mathf.Sign(Input.GetAxis(scrollAxis));
			
			while(currentWeapon >= weapons.Count)currentWeapon-=weapons.Count;
			while(currentWeapon < 0)currentWeapon+=weapons.Count;
			while(!weapons[currentWeapon]){
				currentWeapon += (int)Mathf.Sign(Input.GetAxis(scrollAxis));
				while(currentWeapon >= weapons.Count)currentWeapon-=weapons.Count;
				while(currentWeapon < 0)currentWeapon+=weapons.Count;
			}
			
			weapons[currentWeapon].Enable(true);
		}
		
		// Fire gadget
		if(Input.GetButtonDown(fireKey)){
			BroadcastMessage("OnFire");
		}
		if(Input.GetButton(fireKey)){
			BroadcastMessage("OnFiring");
		}
	}
	public float maxAmmo{
		get { return (weapons[currentWeapon] != null)?weapons[currentWeapon].maxAmmo:0; }
	}
	public float ammo{
		get { return (weapons[currentWeapon] != null)?weapons[currentWeapon].ammo:0; }
	}
	public float maxMags{
		get { return (weapons[currentWeapon] is BaseGun)?(weapons[currentWeapon] as BaseGun).maxMags:0; }
	}
	public float mags{
		get { return (weapons[currentWeapon] is BaseGun)?(weapons[currentWeapon] as BaseGun).mags:0; }
	}
}
