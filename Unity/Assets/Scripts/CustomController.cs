using UnityEngine;
using System.Collections;

public class CustomController : MonoBehaviour {
	public float MouseSensX = 1;
	public float MouseSensY = 1;
	public float MoveForwardSpeed = 0.5f;
	public float MoveSidewaySpeed = 0.5f;
	
	private Camera cam;
	private Vector3 face;

	void Start () {
		cam = transform.GetComponentInChildren<Camera>();
		
		Screen.lockCursor = true;
	}
	void OnMouseDown() {
		Screen.lockCursor = true;
	}
	
	void Update () {
		
		// Rotate player
		transform.Rotate(Vector3.up,Input.GetAxis("Mouse X") * MouseSensX,Space.World);
		transform.Rotate(Vector3.left,Input.GetAxis("Mouse Y") * MouseSensY,Space.Self);
		
		// Move player
		transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * MoveForwardSpeed);
		transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * MoveSidewaySpeed);
		
		// Message actions
	}
}
