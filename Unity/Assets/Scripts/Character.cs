using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	public string jumpKey = "Fire2";
	public float jumpForce = 40;
	
	private Vector3 middleScreen;
	private CharacterMotor motor;
	private bool hanging;

	void Start () {
		middleScreen = new Vector3(Screen.width/2,Screen.height/2,0);
		motor = GetComponentInChildren<CharacterMotor>();
		hanging = false;
	}
	
	void Update () {
		Screen.lockCursor = true;
		
		if(hanging){
			motor.SetVelocity(Vector3.zero);
		}
		if(Input.GetButtonDown(jumpKey) && (motor.grounded || hanging)){
			motor.SetVelocity(GetComponentInChildren<Camera>().ScreenPointToRay(middleScreen).direction*jumpForce);
		}
	}
	void OnExternalVelocity(){
		
	}
}
