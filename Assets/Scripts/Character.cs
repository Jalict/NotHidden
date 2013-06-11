using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(FPSInputController))]

public class Character : MonoBehaviour {
	
	// Settings
	public string fireKey = "Fire1";
	public string jumpKey = "Fire2";
	public string clingKey = "Fire2";
	public string unclingKey = "Jump";
	public string visionKey = "Fire3";
	public float maxEnergy = 100;
	public float energyRegen = 20;
	public float jumpCost = 20;
	public float jumpForce = 40;
	public float clingCost = 25;
	public float visionCost = 30;
	public List<Transform> visions;
	public string unitType;
	
	// Jumping variables
	private Vector3 middleScreen;
	private CharacterMotor motor;
	private FPSInputController fpscon;
	private bool hanging;
	private bool jumping;
	private float gravity;
	private float energy;
	
	// Vision variables
	private Camera cam;
	private Camera visionCam;
	private List<Color> defaultColor;

	void Start () {
		// Setup jumping variables
		middleScreen = new Vector3(Screen.width/2,Screen.height/2,0);
		motor = GetComponentInChildren<CharacterMotor>();
		fpscon = GetComponentInChildren<FPSInputController>();
		gravity = motor.movement.gravity;
		hanging = false;
		jumping = false;
		energy = maxEnergy;
		
		// Setup vision variables
		cam = gameObject.GetComponentsInChildren<Camera>()[0];
		visionCam = gameObject.GetComponentsInChildren<Camera>()[1];
		visionCam.enabled = false;
		defaultColor = new List<Color>();
		int i = visions.Count;
		while(i-- > 0){
			defaultColor.Add(visions[i].renderer.material.color);
			visions[i].gameObject.layer = LayerMask.NameToLayer("HunterVision");
			// TODO check if vision has hp
		}
	}
	
	void Update () {
		// Keep locking the cursor
		Screen.lockCursor = true;
		
		// Regenerate energy
		energy += energyRegen * Time.deltaTime;
		
		// Switch camera mode
		int i = visions.Count;
		if(Input.GetButton(visionKey) && energy>=visionCost*Time.deltaTime){
			energy -= visionCost*Time.deltaTime;
			if(!visionCam.enabled){
				visionCam.enabled = true;
				while(i-- > 0){
					visions[i].renderer.material.color = Color.red;
				}
			}
		} else {
			if(visionCam.enabled){
				visionCam.enabled = false;
				while(i-- > 0){
					visions[i].renderer.material.color = defaultColor[i];
				}
			}
		}
		
		// Cling to walls
		if(hanging){
			if(Input.GetButton(unclingKey) || energy<clingCost*Time.deltaTime){
				hanging = false;
				fpscon.alive = 1;
				motor.movement.gravity = gravity;
			} else {
				energy -= clingCost*Time.deltaTime;
				motor.SetVelocity(Vector3.zero);
				motor.inputMoveDirection = Vector3.zero;
				motor.movement.gravity = 0;
				fpscon.alive = 0;
			}
		}
		// Jump
		if(Input.GetButtonDown(jumpKey) && (motor.grounded || hanging) && energy >= jumpCost){
			hanging = false;
			fpscon.alive = 1;
			motor.movement.gravity = gravity;
			jumping = true;
			energy -= jumpCost;
			motor.SetVelocity(GetComponentInChildren<Camera>().ScreenPointToRay(middleScreen).direction*jumpForce);
		}
		
		// Fire gadget
		if(Input.GetButtonDown(fireKey)){
			
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
	void OnGUI() {
		// Draw energy bar
		// TODO prettier energy bar
		GUI.Box(new Rect(10,Screen.height-30,200+12,20),"");
		GUI.Box(new Rect(10,Screen.height-30,(energy/maxEnergy)*200+12,20),"");
	}
}
