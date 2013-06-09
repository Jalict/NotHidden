using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour {
	public string jumpKey = "Fire2";
	public float jumpForce = 40;
	public float maxEnergy = 100;
	public float energyRegen = 20;
	public float jumpCost = 20;
	
	private Vector3 middleScreen;
	private CharacterMotor motor;
	private bool hanging;
	private bool jumping;
	private float gravity;
	
	private float energy;

	void Start () {
		middleScreen = new Vector3(Screen.width/2,Screen.height/2,0);
		motor = GetComponentInChildren<CharacterMotor>();
		gravity = motor.movement.gravity;
		hanging = false;
		jumping = false;
		
		energy = maxEnergy;
	}
	
	void Update () {
		Screen.lockCursor = true;
		energy += energyRegen * Time.deltaTime;
		if(energy>maxEnergy)energy=maxEnergy;
		
		if(hanging){
			motor.SetVelocity(Vector3.zero);
			motor.inputMoveDirection = Vector3.zero;
			motor.movement.gravity = 0;
		}
		if(Input.GetButtonDown(jumpKey) && (motor.grounded || hanging) && energy >= jumpCost){
			hanging = false;
			motor.movement.gravity = gravity;
			jumping = true;
			energy -= jumpCost;
			motor.SetVelocity(GetComponentInChildren<Camera>().ScreenPointToRay(middleScreen).direction*jumpForce);
		}
		
	}
	void OnExternalVelocity(){
		
	}
	void OnControllerColliderHit(ControllerColliderHit hit){
		if (jumping) {
			Vector3 dir = hit.moveDirection;
			dir.y = Mathf.Max(0,dir.y);
			if (dir.magnitude > 0.1) {
				hanging = true;
			}
		}
		jumping = false;
	}
	void OnGUI() {
		GUI.Box(new Rect(10,Screen.height-30,200+12,20),"");
		GUI.Box(new Rect(10,Screen.height-30,(energy/maxEnergy)*200+12,20),"");
	}
}
