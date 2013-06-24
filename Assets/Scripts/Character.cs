using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(FPSInputController))]

public class Character : MonoBehaviour {
	
	// Key Settings
	public string jumpKey = "Fire2";
	public string clingKey = "Fire2";
	public string unclingKey = "Jump";
	public string visionKey = "Fire3";
	// Energy Settings
	public float maxEnergy = 100;
	public float energyRegen = 20;
	public float jumpCost = 20;
	public float clingCost = 25;
	public float visionCost = 30;
	// Other Settings
	public float jumpForce = 40;
	public List<Transform> visions;
	new public bool active = true;
	
	// Jumping variables
	private Vector3 middleScreen;
	private CharacterMotor motor;
	private bool hanging;
	private bool jumping;
	private float gravity;
	internal float energy;
	
	// Vision variables
	private Camera maincam;
	private Camera visionCam;
	private List<Color> defaultColor;
	
	void Start () {
		// Setup jumping variables
		middleScreen = new Vector3(Screen.width/2,Screen.height/2,0);
		motor = GetComponentInChildren<CharacterMotor>();
		gravity = motor.movement.gravity;
		hanging = false;
		jumping = false;
		energy = maxEnergy;
		
		// Setup vision variables
		maincam = GetComponentsInChildren<Camera>()[0];
		visionCam = GetComponentsInChildren<Camera>()[1];
		visionCam.enabled = false;
		defaultColor = new List<Color>();
		for(int i = 0; i<visions.Count; i++){
			defaultColor.Add(visions[i].GetComponentInChildren<Renderer>().material.color);
			visions[i].GetComponentInChildren<Renderer>().gameObject.layer = LayerMask.NameToLayer("Enemies");
			// TODO check if vision has hp
		}
	}
	
	void Update () {
		// Regenerate energy
		if(motor.grounded)energy += energyRegen * Time.deltaTime;
		
		// Cling to walls
		if(hanging){
			if(Input.GetButton(unclingKey) || energy<clingCost*Time.deltaTime){
				hanging = false;
				motor.SetControllable(true);
				motor.movement.gravity = gravity;
			} else {
				energy -= clingCost*Time.deltaTime;
				motor.SetVelocity(Vector3.zero);
				motor.inputMoveDirection = Vector3.zero;
				motor.movement.gravity = 0;
				motor.SetControllable(false);
			}
		}
		
		if(active){
			// Jump
			if(Input.GetButtonDown(jumpKey) && (motor.grounded || hanging)){
				hanging = false;
				motor.SetControllable(true);
				motor.movement.gravity = gravity;
				jumping = true;
				motor.SetVelocity(maincam.ScreenPointToRay(middleScreen).direction*jumpForce*Mathf.Clamp01(energy/jumpCost));
				energy -= Mathf.Min(jumpCost,energy);
			}
			
			// Switch camera mode
			int i = visions.Count;
			if(Input.GetButton(visionKey) && energy>=visionCost*Time.deltaTime && motor.movement.velocity.magnitude==0){
				energy -= visionCost*Time.deltaTime;
				if(!visionCam.enabled){
					visionCam.enabled = true;
					while(i-- > 0){
						if(visions[i] == null){ // Remove vision if null
							visions.RemoveAt(i);
							continue;
						}
						visions[i].GetComponentInChildren<Renderer>().material.color = Color.red;
					}
				}
			} else {
				if(visionCam.enabled){
					visionCam.enabled = false;
					while(i-- > 0){
						visions[i].GetComponentInChildren<Renderer>().material.color = defaultColor[i];
					}
				}
			}
		}
		
		// Clamp energy
		energy = Mathf.Clamp(energy,0,maxEnergy);
	}
	void OnExternalVelocity(){
		// This is just here to stop an error. Mind it not.
	}
	void OnControllerColliderHit(ControllerColliderHit hit){
		// If wall or roof collision and during jump then cling
		if (jumping) {
			Vector3 dir = hit.moveDirection;
			dir.y = Mathf.Max(0,dir.y);
			if (dir.magnitude > 0.1 && Input.GetButton(clingKey)) {
				hanging = true;
			}
		}
		jumping = false;
	}
}
