using UnityEngine;
using System.Collections;

public class DebugOnly : MonoBehaviour {
	void Start () {
		if(GameObject.Find("Manager"))
			Destroy(gameObject);
	}
}
