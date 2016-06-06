using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupActivator : MonoBehaviour {

    public PowerupAction action;
    public GameObject playerObject;
    public GameObject platformContainer;

    void OnTriggerEnter(Collider other)
    {
        Destroy(transform.gameObject);
        if (action != null && playerObject != null && platformContainer != null) {
            action(playerObject, platformContainer);
        }
    }
}

public delegate void PowerupAction(GameObject playerObject, GameObject platformContainer);
