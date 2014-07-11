using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class WorldGenerator
{
    private Queue<IChunk> _preLoadedTerrainChunks;
    private Queue<IChunk> _preLoadedMeshChunks;
    private IChunk[] _preLoadedTerrainBuffer;
    private IChunk[] _preLoadedMeshBuffer;
    private BiomeController biome;
    private int maxHeight;
    private int chunkThreads;

    public int PreLoadedTerrainCount
    {
        get { return _preLoadedTerrainChunks.Count; }
    }

    public int PreLoadedMeshCount
    {
        get { return _preLoadedMeshChunks.Count; }
    }

    public WorldGenerator(BiomeController biome, int maxHeight, int chunkThreads)
    {
        this.biome = biome;
        this.maxHeight = maxHeight;
        this.chunkThreads = chunkThreads;

        _preLoadedTerrainChunks = new Queue<IChunk>();
        _preLoadedMeshChunks = new Queue<IChunk>();
        _preLoadedTerrainBuffer = new IChunk[chunkThreads];
        _preLoadedMeshBuffer = new IChunk[chunkThreads];

    }

    public void QueueTerrain(IChunk chunk)
    {
        _preLoadedTerrainChunks.Enqueue(chunk);
    }

    public void QueueMesh(IChunk chunk)
    {
        _preLoadedMeshChunks.Enqueue(chunk);
    }

    public IEnumerator ChunkGenerator()
    {
        for (int i = 0; i < chunkThreads; i++)
        {
            Thread t = new Thread(new ParameterizedThreadStart(WorkerThread));
            t.Start(i);
        }
        while (true)
        {
            // For every update move chunks into any index slot that is 
            // free. The threads associated with the indices will then take 
            // this chunk, work on it and free up their slot again.
            for (int i = 0; i < _preLoadedTerrainBuffer.Length; i++)
            {
                if (_preLoadedTerrainBuffer[i] == null)
                {
                    if (_preLoadedTerrainChunks.Count > 0)
                        _preLoadedTerrainBuffer[i] = _preLoadedTerrainChunks.Dequeue();
                }
                if (_preLoadedMeshBuffer[i] == null)
                {
                    if (_preLoadedMeshChunks.Count > 0)
                    {
                        IChunk chunk = _preLoadedMeshChunks.Peek();
                        _preLoadedMeshBuffer[i] = _preLoadedMeshChunks.Dequeue();
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private readonly static object enqueueLock = new object();
    private void WorkerThread(object obj)
    {
        int index = (int)obj;
        while (true)
        {

            if (_preLoadedTerrainChunks.Count > 0)
            {
                // Load Terrains
                if (_preLoadedTerrainBuffer[index] != null)
                {
                    IChunk chunk = _preLoadedTerrainBuffer[index];
                    biome.GenerateChunk(chunk, maxHeight);
                    _preLoadedTerrainBuffer[index] = null;
                    lock (enqueueLock)
                    {
                        _preLoadedMeshChunks.Enqueue(chunk);
                    }
                }
            }
            else
            {

                // Load Meshes
                if (_preLoadedMeshBuffer[index] != null)
                {
                    IChunk chunk = _preLoadedMeshBuffer[index];
                    chunk.ForceCreateMesh();
                    _preLoadedMeshBuffer[index] = null;
                }
            }

            Thread.Sleep(25);
        }
    }

}