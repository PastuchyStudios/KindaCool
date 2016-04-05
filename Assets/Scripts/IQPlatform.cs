﻿using UnityEngine;
using System.Collections;

public class IQPlatform : MonoBehaviour {

    public float forcePerTick = 0.03f;
    public float velocityThreshold = 0.05f;

    private float forceMagnitude = 0;

    private new Rigidbody rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null) {
            foreach (Transform child in transform) {
                rigidbody = child.GetComponent<Rigidbody>();
                if (rigidbody != null) {
                    break;
                }
            }
        }
        if (rigidbody == null) {
            throw new System.Exception("No Rigidbody found");
        }
    }

    void FixedUpdate() {
        if (rigidbody.velocity.magnitude >= velocityThreshold) {
            forceMagnitude += forcePerTick;
            Vector3 force = rigidbody.velocity.normalized * -forceMagnitude;
            rigidbody.AddForce(force);
        }
        else if (forceMagnitude > 0) {
            forceMagnitude = 0;
            rigidbody.velocity = Vector3.zero;
        }
    }
}
