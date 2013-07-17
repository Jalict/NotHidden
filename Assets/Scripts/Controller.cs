using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	public PlayerInput input = new PlayerInput(Vector2.zero,Vector2.zero);
	
	public float sensitivityX = 8;
	public float sensitivityY = 8;
	
	private Transform cam;
	private float rotationY = 0;

	void Start () {
		cam = GetComponentInChildren<Camera>().transform;
	}
	
	void Update () {
		// Rotation
		transform.Rotate(0, input.look.x * sensitivityX, 0);
		
		rotationY += input.look.y * sensitivityY;
		rotationY = Mathf.Clamp(rotationY, -90, 90);
		cam.localEulerAngles = new Vector3(-rotationY, cam.localEulerAngles.y, 0);
		
		// Translation
		Vector3 direction = new Vector3(input.move.x, 0, input.move.y);
		CharacterMotor motor = GetComponent<CharacterMotor>();
		if(motor){
			if(direction != Vector3.zero){
				float length = direction.magnitude;
				length = Mathf.Min(1,length);
				direction = direction.normalized * length * length;
			}
			motor.inputMoveDirection = transform.rotation * direction;
			//motor.inputJump = Input.GetButton("Jump");
		}
	}
}

public struct PlayerInput{
	public Vector2 move;
	public Vector2 look;
	public PlayerInput(Vector2 move, Vector2 look){
		this.move = move;
		this.look = look;
	}
}
