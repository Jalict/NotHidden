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

	// Use this for initialization
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
		
		// Scroll gadgets
		if(Input.GetAxis(scrollAxis) != 0 && weapons.Count > 0){
			weapons[currentWeapon].Enable(false);
			currentWeapon += (int)Mathf.Sign(Input.GetAxis(scrollAxis));
			while(currentWeapon >= weapons.Count)currentWeapon-=weapons.Count;
			while(currentWeapon < 0)currentWeapon+=weapons.Count;
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
}
