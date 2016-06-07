using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions.Must;

public class PlatformGenerator : MonoBehaviour {

    public Transform playerObject;
    public GameObject platformTemplate;
    public GameObject powerupTemplate;
    public float chunkSize = 50;
    public float verticalDistance = 25;
    public float horizontalDistance = 5;
    public float verticalJittering = 10;
    public float horizotnalJittering = 2.4f;
    public int offsettingChunks = 2;
    public int additionalDownOffset = 1;
    public float density = 0.4f;
    public float initialSpeed = 10;

    public float powerupChance = 0.05f;

    private ChunkId currentChunkId;

    private PlatformGeneratorWorker worker;

    void Start() {
        Restart();
    }

	void FixedUpdate() {
        var newChunk = ChunkId.forCoordinates(playerObject.position, this);
        if (!newChunk.Equals(currentChunkId)) {
            currentChunkId = newChunk;
            worker.PlayerChunkId = currentChunkId;        
        }

        List<PlatformStub> stubs = worker.GetPlatformStubs();
        foreach (var stub in stubs) {
            var platform = UnityEngine.Object.Instantiate(platformTemplate, stub.Position, Quaternion.identity) as GameObject;
            platform.transform.SetParent(transform);
            platform.GetComponent<Rigidbody>().velocity = stub.Velocity;
            platform.GetComponent<PlatformRemover>().playerCharacter = playerObject;

            if (stub.SpawnPowerup) {
                var powerup =
                    UnityEngine.Object.Instantiate(powerupTemplate, stub.Position + Vector3.up*1.3f, Quaternion.identity)
                        as GameObject;
                powerup.transform.parent = transform;
                powerup.GetComponent<BoundToPlatform>().platform = platform;;
                platform.AddComponent<PowerupRemover>().powerup = powerup;
                powerup.GetComponent<Renderer>().material.color = Color.red;
                var powerupActivator = powerup.GetComponent<PowerupActivator>();
                powerupActivator.platformContainer = transform.gameObject;
                powerupActivator.playerObject = playerObject.gameObject;
                powerupActivator.action = ((playerObjectject, _) => playerObjectject.GetComponent<Antigravity>().Run());
            }
        }
    }

    void OnDestroy() {
        Debug.Log("Stopping worker thread...");
        worker.Stop();
    }

	public void Restart () {
        currentChunkId = new ChunkId(0, 0, 0, this);

        if (worker != null) {
            worker.Stop();
        }

        worker = new PlatformGeneratorWorker(this);
        worker.Start();
        worker.PlayerChunkId = currentChunkId;
	}
	
}

public class ChunkId {
    private readonly int x;
    private readonly int y;
    private readonly int z;
    private readonly PlatformGenerator generator;

    public int X { get { return x; } }
    public int Y { get { return y; } }
    public int Z { get { return z; } }

    public ChunkId(int x, int y, int z, PlatformGenerator generator) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.generator = generator;
    }

    public Vector3 Position {
        get {
            return new Vector3(x, y, z) * generator.chunkSize;
        }
    }

    public static ChunkId forCoordinates(Vector3 position, PlatformGenerator generator) {
        var ch = position / generator.chunkSize;
        return new ChunkId((int)Mathf.Round(ch.x), (int)Mathf.Round(ch.y), (int)Mathf.Round(ch.z), generator);
    }

    public override bool Equals(object that) {
        if (that == null) {
            return false;
        }

        var ch = that as ChunkId;
        if (ch == null) {
            return false;
        }

        return ch.X == X && ch.Y == Y && ch.Z == Z;
    }

    public override int GetHashCode() {
        return x * 37 + y * 31 + z * 13;
    }

    public override string ToString() {
        return String.Format("({0} {1} {2})", X, Y, Z);
    }
}
