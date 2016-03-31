using UnityEngine;
using System.Collections;
using System;

public class LevelGenerator : MonoBehaviour {

    public float platformDistance = 10;
    public float levelDistance = 20;
    public float depth = 500;
    public int radius = 5;

    public Transform playerObject;
    public GameObject platformTemplate;

    private float lastDepth;

    void Start() {
        lastDepth = levelDistance;

        generatePlatforms();
    }

    void FixedUpdate() {
        generatePlatforms();
    }

    private void generatePlatforms() {
        float expectedDepth = playerObject.position.y - depth;
        if (lastDepth <= expectedDepth) {
            return;
        }

        float levelHeight = Mathf.Floor(lastDepth / levelDistance) * levelDistance;
        levelHeight -= levelDistance;

        while (levelHeight >= expectedDepth) {
            generateLevel(levelHeight);
            levelHeight -= levelDistance;
        }

        lastDepth = expectedDepth;
    }

    private void generateLevel(float levelHeight) {
        Boolean even = Mathf.Round(levelHeight / levelDistance) % 2 == 0;
        float offset = even ? 0 : (platformDistance / 2);

        for (int y = -radius; y <= radius; y++) {
            for (int x = -radius; x <= radius;  x++) {
                var position = new Vector3(x * platformDistance + offset, levelHeight, y * platformDistance + offset);
                GameObject platformInstance = Instantiate(platformTemplate, position, Quaternion.identity) as GameObject;
                platformInstance.transform.SetParent(transform);
            }
        }
    }
}
