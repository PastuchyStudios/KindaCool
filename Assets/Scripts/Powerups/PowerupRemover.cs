using System.Collections;
using UnityEngine;

public class PowerupRemover : MonoBehaviour {

    public GameObject powerup;

    void OnDestroy() {
        if (powerup != null) {
            Destroy(powerup);
        }
    }
}
