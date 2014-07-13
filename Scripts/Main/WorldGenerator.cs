using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class WorldGenerator
{
    private Queue<IChunk> _preLoadedTerrainChunks;
    private Dictionary<IntVector3, IChunk> _preLoadedMeshChunks;
    private IChunk[] _preLoadedTerrainBuffer;
    private IChunk[] _preLoadedMeshBuffer;
    private World _world;
    private int chunkThreads;

    public int PreLoadedTerrainCount
    {
        get { return _preLoadedTerrainChunks.Count; }
    }

    public int PreLoadedMeshCount
    {
        get { return _preLoadedMeshChunks.Count; }
    }

    public bool Running { get; set; }

    public WorldGenerator(World world, int chunkThreads)
    {
        Running = true;
        this.chunkThreads = chunkThreads;
        this._world = world;
        _preLoadedTerrainChunks = new Queue<IChunk>();
        _preLoadedMeshChunks = new Dictionary<IntVector3, IChunk>();
        _preLoadedTerrainBuffer = new IChunk[chunkThreads];
        _preLoadedMeshBuffer = new IChunk[chunkThreads];

    }

    public void GenerateChunk(IChunk chunk)
    {
        // If the terrain has not been created for the chunk, queue it to be loaded
        if (!chunk.IsLoaded)
            _preLoadedTerrainChunks.Enqueue(chunk);
        if (!_preLoadedMeshChunks.ContainsKey(chunk.ChunkIndex))
            _preLoadedMeshChunks.Add(chunk.ChunkIndex, chunk);
    }

    public IEnumerator ChunkLoader()
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
                if (_preLoadedMeshBuffer[i] == null && _preLoadedTerrainChunks.Count == 0)
                {
                    foreach (IChunk chunk in _preLoadedMeshChunks.Values)
                    {
                        _preLoadedMeshBuffer[i] = chunk;
                        _preLoadedMeshChunks.Remove(chunk.ChunkIndex);
                        break;
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private bool ValidateChunk(IChunk chunk)
    {
        if (chunk.TopChunk == null || chunk.BottomChunk == null || chunk.ForwardChunk == null || chunk.BehindChunk == null || chunk.LeftChunk == null || chunk.RightChunk == null)
            return true;
        return chunk.TopChunk.IsLoaded && chunk.BottomChunk.IsLoaded && chunk.LeftChunk.IsLoaded && chunk.RightChunk.IsLoaded && chunk.ForwardChunk.IsLoaded && chunk.BehindChunk.IsLoaded;
    }

    /// <summary>
    /// Create chunk terrain and meshes. Each threads loads a specific or mesh based on an index value of that thread.
    /// Objects are moved into arrays from the ChunkLoader method.
    /// </summary>
    /// <param name="obj"></param>
    private void WorkerThread(object obj)
    {
        int index = (int)obj;
        while (Running)
        {
            Thread.Sleep(25);

            // Load Terrains
            if (_preLoadedTerrainBuffer[index] != null)
            {
                IChunk chunk = _preLoadedTerrainBuffer[index];
                _world.biome.GenerateChunk(chunk, _world.maxHeight);
                _preLoadedTerrainBuffer[index] = null;
            }

            // Load Meshes
            if (_preLoadedMeshBuffer[index] != null)
            {
                IChunk chunk = _preLoadedMeshBuffer[index];
                chunk.ForceCreateMesh();
                _preLoadedMeshBuffer[index] = null;
            }

        }
    }

}