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
		transform.Rotate(0, input.look.x * sensitivityX, 0);
		
		rotationY += input.look.y * sensitivityY;
		rotationY = Mathf.Clamp(rotationY, -90, 90);
		cam.localEulerAngles = new Vector3(-rotationY, cam.localEulerAngles.y, 0);
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
