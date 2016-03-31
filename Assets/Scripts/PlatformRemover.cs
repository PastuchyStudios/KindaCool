using UnityEngine;
using System.Collections;

public class PlatformRemover : MonoBehaviour {
    public Transform playerObject;
    public float removalDistance = 500;

    void FixedUpdate() {
        foreach (Transform platform in transform) {
            var verticalDistance = platform.position.y - playerObject.position.y;
            if (verticalDistance > removalDistance) {
                Destroy(platform.gameObject);
            }
        }
    }
}
