using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour {
	public string jumpKey = "Fire2";
	public float jumpForce = 40;
	
	private Vector3 middleScreen;
	private CharacterMotor motor;
	private bool hanging;
	private bool jumping;
	private float gravity;

	void Start () {
		middleScreen = new Vector3(Screen.width/2,Screen.height/2,0);
		motor = GetComponentInChildren<CharacterMotor>();
		gravity = motor.movement.gravity;
		hanging = false;
		jumping = false;
	}
	
	void Update () {
		Screen.lockCursor = true;
		
		if(hanging){
			motor.SetVelocity(Vector3.zero);
			motor.inputMoveDirection = Vector3.zero;
			motor.movement.gravity = 0;
		}
		if(Input.GetButtonDown(jumpKey) && (motor.grounded || hanging)){
			hanging = false;
			motor.movement.gravity = gravity;
			jumping = true;
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
}
