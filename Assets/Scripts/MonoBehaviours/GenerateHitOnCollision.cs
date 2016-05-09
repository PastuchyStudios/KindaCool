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
        ForceReceiver forceReceiver = hit.gameObject.GetComponentInChildren(typeof(ForceReceiver), false) as ForceReceiver;
        if (forceReceiver == null) {
            return;
        }

        AppliedForce hitForce = new AppliedForce(hit.point, lastVelocity.magnitude * velocityToForceFactor);
        forceReceiver.receiveHit(hitForce);
        lastVelocity = Vector3.zero;
    }
}
