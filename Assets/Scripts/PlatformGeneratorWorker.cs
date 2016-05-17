using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class PlatformGeneratorWorker {

    private ChunkId playerChunkId = null;
    private object playerPositionLock = new object();
    private bool playerChunkChanged = false;

    private List<PlatformStub> platformStubs = new List<PlatformStub>();
    private object resultsLock = new object();

    private bool working = true;
    private object workLock = new object();

    private PlatformGenerator generator;
    private readonly int offsettingChunks;
    private readonly int additionalDownOffset;
    private HashSet<ChunkId> generatedChunks = new HashSet<ChunkId>();
    private Queue<ChunkId> chunksBeingGenerated = new Queue<ChunkId>();

    public PlatformGeneratorWorker(PlatformGenerator generator) {
        this.generator = generator;
        this.offsettingChunks = generator.offsettingChunks;
        this.additionalDownOffset = generator.additionalDownOffset;
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
            try {
                Step();
            } catch (Exception e) {
                Debug.Log(e);
            }
        }
    }

    private void Step() {
        ChunkId chunkToDo = null;
        if (PlayerChunkChanged) {
            ChunkId centerChunk = PlayerChunkId;
            HashSet<ChunkId> newVisible = new HashSet<ChunkId>(localChunks(centerChunk.X, centerChunk.Y, centerChunk.Z));

            chunksBeingGenerated = new Queue<ChunkId>(newVisible.Where(e => !generatedChunks.Contains(e)).Where(e => !e.Equals(centerChunk)));
            chunkToDo = generatedChunks.Contains(centerChunk) ? null : centerChunk;
        } else if (chunksBeingGenerated.Any()) {
            chunkToDo = chunksBeingGenerated.Dequeue();
        }

        if (chunkToDo != null) {
            AddPlatformsToGenerate(GeneratePlatforms(chunkToDo));
            generatedChunks.Add(chunkToDo);
        }
    }

    private List<PlatformStub> GeneratePlatforms(ChunkId chunkToDo) {
        List<PlatformStub> result = new List<PlatformStub>();
        float horizontalStart = -generator.chunkSize / 2 + generator.horizontalDistance / 2;
        float verticalStart = -generator.chunkSize / 2 + generator.verticalDistance / 2;

        System.Random random = new System.Random(chunkToDo.GetHashCode());

        for (float x = horizontalStart; x < generator.chunkSize / 2; x += generator.horizontalDistance) {
            for (float y = verticalStart; y < generator.chunkSize / 2; y += generator.verticalDistance) {
                for (float z = horizontalStart; z < generator.chunkSize / 2; z += generator.horizontalDistance) {
                    if (random.NextDouble() < generator.density) {
                        var position = chunkToDo.Position + new Vector3(x, y, z) + RandomShift(random);
                        var velocity = RandomVelocity(random);
                        result.Add(new PlatformStub { Position = position, Velocity = velocity });
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

    private Vector3 RandomVelocity(System.Random random) {
        float phi = RandomFromRange(random, 0.0f, (float) Math.PI);
        float z = RandomFromRange(random, -1, 1);
        float theta = (float) Math.Acos(z);

        float x = (float)(Math.Sin(theta) * Math.Cos(phi));
        float y = (float)(Math.Sin(theta) * Math.Sin(phi));
        return new Vector3(x, y, z) * RandomFromRange(random, 0, generator.initialSpeed);
    }

    private float RandomFromRange(System.Random random, float min, float max) {
        return (float) (min + random.NextDouble() * (max - min));
    }

    private IEnumerable<ChunkId> localChunks(int x0, int y0, int z0) {
        for (int x = -offsettingChunks; x <= offsettingChunks; x++) {
            for (int y = -offsettingChunks - additionalDownOffset; y <= offsettingChunks; y++) {
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

    public List<PlatformStub> GetPlatformStubs() {
        List<PlatformStub> temp;
        lock (resultsLock) {
            temp = platformStubs;
            platformStubs = new List<PlatformStub>();
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

    private void AddPlatformsToGenerate(IList<PlatformStub> stubs) {
        lock (resultsLock) {
            platformStubs.AddRange(stubs);
        }
    }
   }

public class PlatformStub {
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }
}
