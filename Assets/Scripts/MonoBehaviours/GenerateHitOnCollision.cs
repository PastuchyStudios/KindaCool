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
    
    void Start() {
        characterController = GetComponent<CharacterController>();
        scoreCounter = GetComponent<ScoreCounter>();
        position = characterController.transform.position;
    }

    void FixedUpdate() {
        lastVelocity = characterController.velocity;
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        ForceReceiver forceReceiver = hit.gameObject.GetComponentInChildren(typeof(ForceReceiver), false) as ForceReceiver;
        if (forceReceiver == null) {
            return;
        }

        float forceMagnitude = (lastVelocity - hit.rigidbody.velocity).magnitude * velocityToForceFactor;
        
        AppliedForce hitForce = new AppliedForce(hit.point, forceMagnitude);
        forceReceiver.receiveHit(hitForce);
        
        if (forceMagnitude > durability) {
            restartScreen.GetComponent<RestartScreen>().enable(scoreCounter.score);
        } else {
            scoreCounter.updateScore();
        }
        
        lastVelocity = Vector3.zero;
        
    }
}
