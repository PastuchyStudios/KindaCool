using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class PlatformGeneratorWorker {

    private ChunkId playerChunkId = null;
    private object playerPositionLock = new object();
    private bool playerChunkChanged = false;

    private IList<ChunkChange> chunkChanges = new List<ChunkChange>();
    private object resultsLock = new object();

    private bool working = true;
    private object workLock = new object();

    private PlatformGenerator generator;
    private readonly int offsettingChunks;
    private HashSet<ChunkId> visibleChunks = new HashSet<ChunkId>();
    private Queue<ChunkId> chunksBeingGenerated = new Queue<ChunkId>();

    public PlatformGeneratorWorker(PlatformGenerator generator) {
        this.generator = generator;
        this.offsettingChunks = generator.offsettingChunks;
    }

    public void Start() {
        new Thread(Work).Start();
    }

    public void Stop() {
        Working = false;
    }

    private void Work() {
        while (Working) {
            // https://www.youtube.com/watch?v=KlujizeNNQM
            Step();
        }
    }

    private void Step() {
        ChunkId chunkToDo = null;
        if (PlayerChunkChanged) {
            ChunkId centerChunk = PlayerChunkId;
            HashSet<ChunkId> newVisible = new HashSet<ChunkId>(localChunks(centerChunk.X, centerChunk.Y, centerChunk.Z));
            foreach (var id in visibleChunks.Where(e => !newVisible.Contains(e))) {
                AddChunkToRemove(id);
            }
            chunksBeingGenerated = new Queue<ChunkId>(newVisible.Where(e => !visibleChunks.Contains(e)).Where(e => e != centerChunk));
            chunkToDo = centerChunk;
        } else if (chunksBeingGenerated.Any()) {
            chunkToDo = chunksBeingGenerated.Dequeue();
        }

        if (chunkToDo != null) {
            AddPlatformsToGenerate(chunkToDo, GeneratePlatforms(chunkToDo));
            visibleChunks.Add(chunkToDo);
        }
    }

    private IList<Vector3> GeneratePlatforms(ChunkId chunkToDo) {
        List<Vector3> result = new List<Vector3>();
        float horizontalStart = -generator.chunkSize / 2 + generator.horizontalDistance / 2;
        float verticalStart = -generator.chunkSize / 2 + generator.verticalDistance / 2;

        System.Random random = new System.Random(chunkToDo.GetHashCode());

        for (float x = horizontalStart; x < generator.chunkSize / 2; x += generator.horizontalDistance) {
            for (float y = verticalStart; y < generator.chunkSize / 2; y += generator.verticalDistance) {
                for (float z = horizontalStart; z < generator.chunkSize / 2; z += generator.horizontalDistance) {
                    if (random.NextDouble() < generator.density) {
                        var position = chunkToDo.Position + new Vector3(x, y, z) + RandomShift(random);
                        result.Add(position);
                    }
                }
            }
        }
        return result;
    }

    private Vector3 RandomShift(System.Random random) {
        return new Vector3(
            RandomFromRange(random, -generator.horizotnalJittering, generator.horizotnalJittering),
            RandomFromRange(random, -generator.verticalJittering, generator.verticalJittering),
            RandomFromRange(random, -generator.horizotnalJittering, generator.horizotnalJittering)
        );
    }

    private float RandomFromRange(System.Random random, float min, float max) {
        return (float) (min + random.NextDouble() * (max - min));
    }

    private IEnumerable<ChunkId> localChunks(int x0, int y0, int z0) {
        for (int x = -offsettingChunks; x <= offsettingChunks; x++) {
            for (int y = -offsettingChunks; y <= offsettingChunks; y++) {
                for (int z = -offsettingChunks; z <= offsettingChunks; z++) {
                    yield return new ChunkId(x + x0, y + y0, z + z0, generator);
                }
            }
        }
    }

    public ChunkId PlayerChunkId {
        private get {
            ChunkId temp;
            lock (playerPositionLock) {
                playerChunkChanged = false;
                temp = playerChunkId;
            }
            return temp;
        }
        set {
            lock (playerPositionLock) {
                playerChunkId = value;
                playerChunkChanged = true;
            }
        }
    }

    private bool Working {
        get {
            bool temp;
            lock (workLock) {
                temp = working;
            }
            return temp;
        }
        set {
            lock (workLock) {
                working = value;
            }
        }
    }

    public IList<ChunkChange> GetChanges() {
        IList<ChunkChange> temp;
        lock (resultsLock) {
            temp = chunkChanges;
            chunkChanges = new List<ChunkChange>();
        }
        return temp;
    }

    private bool PlayerChunkChanged {
         get {
            bool temp;
            lock (playerPositionLock) {
                temp = playerChunkChanged;
            }
            return temp;
        }
    }

    private void AddPlatformsToGenerate(ChunkId chunkId, IList<Vector3> positions) {
        lock (resultsLock) {
            chunkChanges.Add(ChunkChange.Added(chunkId, positions));
        }
    }

    private void AddChunkToRemove(ChunkId chunkId) {
        lock (resultsLock) {
            chunkChanges.Add(ChunkChange.Removed(chunkId));
        }
    }

}

public struct ChunkChange {
    public readonly ChunkId ChunkId;
    public readonly IList<Vector3> AddedElements;
    public readonly bool WasRemoved;

    private ChunkChange(ChunkId chunkId, IList<Vector3> added, bool wasRemoved) {
        this.ChunkId = chunkId;
        this.AddedElements = added;
        this.WasRemoved = wasRemoved;
    }

    public static ChunkChange Removed(ChunkId chunkId) {
        return new ChunkChange(chunkId, new List<Vector3>(), true); 
    }

    public static ChunkChange Added(ChunkId chunkId, IList<Vector3> added) {
        return new ChunkChange(chunkId, added, false);
    }
}
