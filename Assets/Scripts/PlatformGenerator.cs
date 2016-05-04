using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlatformGenerator : MonoBehaviour {

    public Transform playerObject;
    public GameObject platformTemplate;
    public float chunkSize = 50;
    public float verticalDistance = 25;
    public float horizontalDistance = 5;
    public float verticalJittering = 10;
    public float horizotnalJittering = 2.4f;
    public int offsettingChunks = 2;
    public int additionalDownOffset = 1;
    public float density = 0.4f;

    private IDictionary<ChunkId, Chunk> chunks;
    private ChunkId currentChunkId;

    private PlatformGeneratorWorker worker;

	void Start () {
        currentChunkId = new ChunkId(0, 0, 0, this);
        chunks = new Dictionary<ChunkId, Chunk>();

        worker = new PlatformGeneratorWorker(this);
        worker.Start();
        worker.PlayerChunkId = currentChunkId;
	}
	
	void FixedUpdate() {
        var newChunk = ChunkId.forCoordinates(playerObject.position, this);
        if (!newChunk.Equals(currentChunkId)) {
            currentChunkId = newChunk;
            worker.PlayerChunkId = currentChunkId;        
        }

        IList<ChunkChange> changes = worker.GetChanges();
        foreach (var change in changes) {
            if (change.WasRemoved) {
                chunks[change.ChunkId].Remove();
                chunks.Remove(change.ChunkId);
            } else {
                chunks.Add(change.ChunkId, new Chunk(this, change.AddedElements));
            }
        }           
	}

    void OnDestroy() {
        Debug.Log("Stopping worker thread...");
        worker.Stop();
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

class Chunk {
    private readonly List<GameObject> platforms;
    private readonly PlatformGenerator generator;

    public Chunk(PlatformGenerator generator, IList<Vector3> positions) {
        platforms = new List<GameObject>();
        this.generator = generator;

        generateChunkPlatforms(positions);
    }

    public void Remove() {
        foreach (GameObject platform in platforms) {
            UnityEngine.Object.Destroy(platform);
        }
    }

    private void generateChunkPlatforms(IList<Vector3> positions) {
        foreach (var position in positions) {
            var platform = UnityEngine.Object.Instantiate(generator.platformTemplate, position, Quaternion.identity) as GameObject;
            platform.transform.SetParent(generator.transform);
            platforms.Add(platform);
        }
    }
}
