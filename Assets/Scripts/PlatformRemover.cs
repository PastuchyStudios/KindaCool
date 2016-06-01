using UnityEngine;
using System.Collections;

public class PlatformRemover : MonoBehaviour {

    public Transform playerCharacter = null;
    public float horizontalDistance;
    public float upDistance;
    public float downDistance;

    void FixedUpdate() {
        if (playerCharacter != null) {
            Vector2 playerFlatPosition = new Vector2(playerCharacter.position.x, playerCharacter.position.z);
            Vector2 platformFlatPosition = new Vector2(transform.position.x, transform.position.z);
            float upperBound = playerCharacter.position.y + upDistance;
            float lowerBound = playerCharacter.position.y - downDistance; 
            float platformHeight = transform.position.y;
           

            if (Vector2.Distance(playerFlatPosition, platformFlatPosition) > horizontalDistance
                || platformHeight > upperBound || platformHeight < lowerBound) {
                Destroy(gameObject);
            }
        }
    }
}
