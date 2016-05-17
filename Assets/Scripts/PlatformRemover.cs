using UnityEngine;
using System.Collections;

public class PlatformRemover : MonoBehaviour {

    public Transform playerCharacter;
    public float horizontalDistance;
    public float verticalDistance;

    void FixedUpdate() {
        Vector2 playerFlatPosition = new Vector2(playerCharacter.position.x, playerCharacter.position.z);
        Vector2 platformFlatPosition = new Vector2(transform.position.x, transform.position.z);
        float playerHeight = playerCharacter.position.y;
        float shardHeight = transform.position.y;

        if (Vector2.Distance(playerFlatPosition, platformFlatPosition) > horizontalDistance
            || Mathf.Abs(playerHeight - shardHeight) > verticalDistance) {
            // Destroy(gameObject);
        }
    }
}
