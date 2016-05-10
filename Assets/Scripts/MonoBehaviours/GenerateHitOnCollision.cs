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

    private CharacterController characterControler;

    private ScoreCounter scoreCounter; 
    
    private Vector3 position; 
    
    void Start() {
        characterControler = GetComponent<CharacterController>();
        scoreCounter = GetComponent<ScoreCounter>();
        position = characterControler.transform.position;
    }

    void FixedUpdate() {
        lastVelocity = characterControler.velocity;
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        ForceReceiver forceReceiver = hit.gameObject.GetComponentInChildren(typeof(ForceReceiver), false) as ForceReceiver;
        if (forceReceiver == null) {
            return;
        }

        float forceMagnitude = lastVelocity.magnitude * velocityToForceFactor;
        
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
