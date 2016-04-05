using UnityEngine;
using System.Collections;

public class ReturnToTop : MonoBehaviour {
	public Transform playerObject;

	void Update() {
		if (Input.GetKeyDown(KeyCode.F1)) {
			print("Moved to top.");
			playerObject.position = new Vector3(0, 10, 0);
		}
	}
}