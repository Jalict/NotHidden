using UnityEngine;
using System.Collections;

public class CustomController : MonoBehaviour {
	public float MouseSensX = 50f;
	public float MouseSensY = 50f;
	public float MoveForwardSpeed = 20f;
	public float MoveSidewaySpeed = 20f;
	
	private Camera cam;
	private Vector3 face;

	void Start () {
		cam = transform.GetComponentInChildren<Camera>();
		
		Screen.lockCursor = true;
	}
	
	void Update () {
		// Keep locking cursor
		if(Input.GetMouseButtonDown(0)){
			Screen.lockCursor = true;
		}
		
		// Rotate player
		transform.Rotate(Vector3.up,Input.GetAxis("Mouse X") * MouseSensX * Time.deltaTime,Space.World);
		cam.transform.Rotate(Vector3.left,Input.GetAxis("Mouse Y") * MouseSensY * Time.deltaTime,Space.Self);
		
		// Move player
		transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * MoveForwardSpeed * Time.deltaTime);
		transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * MoveSidewaySpeed * Time.deltaTime);
		
		
	}
}
