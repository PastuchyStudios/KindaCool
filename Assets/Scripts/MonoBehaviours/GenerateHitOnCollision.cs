using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class GenerateHitOnCollision : MonoBehaviour {

    public GameObject restartScreen;

    public float velocityToForceFactor = 1;

    public float durability = 40;

    private Vector3 lastVelocity;

    private CharacterController characterController;

    private ScoreCounter scoreCounter;

    private Vector3 position;

    private Transform groundObject;

    private Vector3 lastGroundPosition;

    void Start() {
        characterController = GetComponent<CharacterController>();
        scoreCounter = GetComponent<ScoreCounter>();
        position = characterController.transform.position;
    }

    void FixedUpdate() {
        lastVelocity = characterController.velocity;
        if (!characterController.isGrounded) {
            groundObject = null;
        } else if (groundObject != null) {
            var groundPositionDelta = groundObject.position - lastGroundPosition;
            groundPositionDelta = new Vector3(groundPositionDelta.x, groundPositionDelta.y - 0.01f, groundPositionDelta.z);
            characterController.Move(groundPositionDelta);
            lastGroundPosition = groundObject.position;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        groundObject = hit.transform;
        lastGroundPosition = groundObject.position;

        ForceReceiver forceReceiver = hit.gameObject.GetComponentInChildren(typeof(ForceReceiver), false) as ForceReceiver;
        if (forceReceiver == null) {
            return;
        }

        float forceMagnitude = (lastVelocity - hit.rigidbody.velocity).magnitude * velocityToForceFactor;

        AppliedForce hitForce = new AppliedForce(hit.point, forceMagnitude);
        forceReceiver.receiveHit(hitForce);

        if (forceMagnitude > durability) {
            restartScreen.gameObject.SetActive(true);
        } else {
            scoreCounter.updateScore();
        }

        lastVelocity = Vector3.zero;
    }
}
