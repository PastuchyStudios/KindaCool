using UnityEngine;
using System.Collections;

public class BoundToPlatform : MonoBehaviour
{

    public GameObject platform;
    public float height = 1.3f;

	void FixedUpdate () {
	    if (platform != null) {
	        transform.position = platform.transform.position + Vector3.up * height;
	    }
	}
}
