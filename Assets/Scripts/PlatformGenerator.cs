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
    public float density = 0.75f;

    private List<Chunk> chunks;
    private ChunkId currentChunkId;

	void Start () {
        chunks = localChunks(0, 0, 0).Select(ch => new Chunk(ch, this)).ToList();

        currentChunkId = new ChunkId(0, 0, 0, this);
        
	}
	
	void FixedUpdate () {
        var newChunk = ChunkId.forCoordinates(playerObject.position, this);
        if (!newChunk.Equals(currentChunkId)) {
            currentChunkId = newChunk;
            
            //I know it is ugly as fuck, but when I use HashSet it refuses to compile.
            Dictionary<ChunkId, bool> newVisible = localChunks(newChunk.X, newChunk.Y, newChunk.Z).ToDictionary(ch => ch, ch => true);

            var partition = chunks.ToLookup(chunk => newVisible.ContainsKey(chunk.Id));
            partition[false].ToList().ForEach(chunk => chunk.remove());
            chunks = partition[true].ToList();

            chunks.ForEach(chunk => newVisible.Remove(chunk.Id));
            foreach (var chunkId in newVisible.Keys) {
                chunks.Add(new Chunk(chunkId, this));
            }
        }
        
	}

    private IEnumerable<ChunkId> localChunks(int x0, int y0, int z0) {
        for (int x = -offsettingChunks; x <= offsettingChunks; x++) {
            for (int y = -offsettingChunks; y <= offsettingChunks; y++) {
                for (int z = -offsettingChunks; z <= offsettingChunks; z++) {
                    yield return new ChunkId(x + x0, y + y0, z + z0, this);
                }
            }
        }
    }
}

class ChunkId {
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
        return x * 37 + y * 31 + x * 13;
    }
}

class Chunk {
    private readonly List<GameObject> platforms;
    private readonly ChunkId id;
    private readonly PlatformGenerator generator;

    public ChunkId Id { get { return id; } }

    public Chunk(ChunkId id, PlatformGenerator generator) {
        platforms = new List<GameObject>();
        this.id = id;
        this.generator = generator;

        generateChunkPlatforms();
    }

    public void remove() {
        foreach (GameObject platform in platforms) {
            UnityEngine.Object.Destroy(platform);
        }
    }

    private void generateChunkPlatforms() {
        float horizontalStart = -generator.chunkSize / 2 + generator.horizontalDistance / 2;
        float verticalStart = -generator.chunkSize / 2 + generator.verticalDistance / 2;

        UnityEngine.Random.seed = Id.GetHashCode();

        for (float x = horizontalStart; x < generator.chunkSize / 2; x += generator.horizontalDistance) {
            for (float y = verticalStart; y < generator.chunkSize / 2; y += generator.verticalDistance) {
                for (float z = horizontalStart; z < generator.chunkSize / 2; z += generator.horizontalDistance) {
                    if (UnityEngine.Random.value < generator.density) {
                        var position = id.Position + new Vector3(x, y, z) + randomShift();
                        var platform = UnityEngine.Object.Instantiate(generator.platformTemplate, position, Quaternion.identity) as GameObject;
                        platform.transform.SetParent(generator.transform);
                        platforms.Add(platform);
                    }
                }
            }
        }
    }

    private Vector3 randomShift() {
        return new Vector3(
            UnityEngine.Random.Range(-generator.horizotnalJittering, generator.horizotnalJittering),
            UnityEngine.Random.Range(-generator.verticalJittering, generator.verticalJittering),
            UnityEngine.Random.Range(-generator.horizotnalJittering, generator.horizotnalJittering)
        );
    }
}
