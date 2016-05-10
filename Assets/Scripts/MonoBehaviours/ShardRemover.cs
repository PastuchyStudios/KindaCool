using UnityEngine;
using System.Collections;

public class ShardRemover : MonoBehaviour {

    public Transform playerCharacter;
    public string shardTag = "Platform Shard";
    public float horizontalDistance;
    public float verticalDistance;

    void FixedUpdate() {
        Vector2 playerFlatPosition = new Vector2(playerCharacter.position.x, playerCharacter.position.z);
        float playerHeight = playerCharacter.position.y;

        foreach (Transform child in transform) {
            if (!child.CompareTag(shardTag)) {
                continue;
            } else if (!child.GetComponent<BreakablePlatform>().dead) {
                continue;
            }


            Vector2 shardFlatPosition = new Vector2(child.position.x, child.position.z);
            float shardHeight = child.position.y;

            if (Vector2.Distance(playerFlatPosition, shardFlatPosition) > horizontalDistance
                || Mathf.Abs(playerHeight - shardHeight) > verticalDistance) {
                Destroy(child.gameObject);
            }
        }
    }
}
