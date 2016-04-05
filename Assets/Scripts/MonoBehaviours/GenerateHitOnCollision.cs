using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class GenerateHitOnCollision : MonoBehaviour {

    public float velocityToForceFactor = 1;

    private Vector3 lastVelocity;

    private CharacterController cc;

    void Start() {
        cc = GetComponent<CharacterController>();
    }

    void FixedUpdate() {
        lastVelocity = cc.velocity;
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        Breakable breakable = hit.gameObject.GetComponentInChildren(typeof(Breakable), false) as Breakable;
        if (breakable == null) {
            return;
        }

        PlatformHit platformHit = new PlatformHit(hit.point, lastVelocity.magnitude * velocityToForceFactor);
        breakable.receiveHit(platformHit);
    }
}
