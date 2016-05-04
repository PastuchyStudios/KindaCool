using UnityEngine;
using System.Collections;

public class InertPlatform : MonoBehaviour {

    public float initialBrakingForceFactor = 1f;
    public float brakingForceIncreaseFactor = 0f;
    public float velocityThreshold = 0.05f;

    private float accumulatedFactor = 0;

    private new Rigidbody rigidbody;

    void Start() {
        rigidbody = GetComponentInChildren(typeof(Rigidbody), false) as Rigidbody;
    }

    void FixedUpdate() {
        if (rigidbody.velocity.magnitude >= velocityThreshold) {
            accumulatedFactor += brakingForceIncreaseFactor;
            float totalFactor = initialBrakingForceFactor + accumulatedFactor;
            Vector3 force = rigidbody.velocity.normalized * -totalFactor;
            rigidbody.AddForce(force);
        }
        else {
            accumulatedFactor = 0;
            rigidbody.velocity = Vector3.zero;
        }
    }
}
