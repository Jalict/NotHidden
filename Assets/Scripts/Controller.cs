using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	public bool paused = false;
	public float sensitivityX = 1;
	public float sensitivityY = 1;
	public Vector3 moveInput = Vector3.zero;
	
	private Transform cam;
	private CharacterMotor motor;
	private WeaponHolder weapon;
	private Hunter hunter;
	private float rotationY = 0;

	void Start () {
		cam = GetComponentInChildren<Camera>().transform;
		motor = GetComponent<CharacterMotor>();
		weapon = GetComponent<WeaponHolder>();
		hunter = GetComponent<Hunter>();
	}
	
	void Update () {
		if(!paused){
			// Rotation
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp(rotationY, -90, 90);
			cam.localEulerAngles = new Vector3(-rotationY, cam.localEulerAngles.y, 0);
			
			// Translation
			moveInput = new Vector3(Input.GetAxis("LeftRight"), 0, Input.GetAxis("BackwardForward"));
			if(motor){
				if(moveInput != Vector3.zero){
					float length = moveInput.magnitude;
					length = Mathf.Min(1,length);
					moveInput = moveInput.normalized * length * length;
				}
				motor.inputMoveDirection = transform.rotation * moveInput;
				motor.inputJump = Input.GetButtonDown("Jump");
			}
			
			// Weapon
			if(weapon){
				weapon.inputFire = Input.GetButtonDown("Fire");
				weapon.inputFiring = Input.GetButton("Fire");
				weapon.inputAltFire = Input.GetButtonDown("Alternate Fire");
				weapon.inputAltFiring = Input.GetButton("Alternate Fire");
				weapon.inputScroll = Input.GetAxis("Scroll Weapons");
			}
			
			// Hunter Actions
			if(hunter){
				hunter.inputJump = Input.GetButtonDown("Hunter Leap");
				hunter.inputCling = Input.GetButton("Hunter Leap");
				hunter.inputUncling = Input.GetButton("Jump");
				hunter.inputVision = Input.GetButton("Hunter Vision");
			}
		} else {
			// Translation
			if(motor){
				motor.inputMoveDirection = Vector3.zero;
				motor.inputJump = false;
			}
			// Weapon
			if(weapon){
				weapon.inputFire = false;
				weapon.inputFiring = false;
				weapon.inputAltFire = false;
				weapon.inputAltFiring = false;
				weapon.inputScroll = 0;
			}
			// Hunter Actions
			if(hunter){
				hunter.inputJump = false;
				hunter.inputCling = false;
				hunter.inputUncling = false;
				hunter.inputVision = false;
			}
		}
	}
}
