using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class WorldGenerator
{
    /// <summary>
    /// Queue of all the terrains that are to be loaded. Terrain loading populates the blocks
    /// and does not handle the actual rendering
    /// </summary>
    private Queue<IChunk> _preLoadedTerrainChunks;
    /// <summary>
    /// Dictionary of all chunk meshes that are to be created. For a mesh to be created the surrounding 
    /// chunks must first be loaded via the biome controller. Subsequent updates to chunks will also be added
    /// this this Dictionary.
    /// </summary>
    private Dictionary<IntVector3, IChunk> _preLoadedMeshChunks;
    /// <summary>
    /// To avoid multiple threads attempting to simultaneously create the same chunk mesh, chunks currently being
    /// operated on are added / removed here synchronously.
    /// </summary>
    private Dictionary<IntVector3, IChunk> _loadingMeshChunks;
    private IChunk[] _preLoadedTerrainBuffer;
    private IChunk[] _preLoadedMeshBuffer;
    private World _world;
    private int chunkThreads;

    private static readonly object meshChunkLock = new object();

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
        _loadingMeshChunks = new Dictionary<IntVector3, IChunk>();
        _preLoadedTerrainBuffer = new IChunk[chunkThreads];
        _preLoadedMeshBuffer = new IChunk[chunkThreads];

    }

    public void GenerateChunk(IChunk chunk)
    {
        // If the terrain has not been created for the chunk, queue it to be loaded
        if (!chunk.IsLoaded)
            _preLoadedTerrainChunks.Enqueue(chunk);
        lock (meshChunkLock) { 
        if (!_preLoadedMeshChunks.ContainsKey(chunk.ChunkIndex))
            _preLoadedMeshChunks.Add(chunk.ChunkIndex, chunk);
            }
    }

    readonly static object loadLock = new object();
    bool AddLoadingMeshChunk(IChunk chunk)
    {
        lock (loadLock)
        {
            if (!_loadingMeshChunks.ContainsKey(chunk.ChunkIndex))
            {
                _loadingMeshChunks.Add(chunk.ChunkIndex, chunk);
                return true;
            }
            return false;
        }
    }

    void RemoveLoadingMeshChunk(IChunk chunk)
    {
        lock (loadLock)
        {
            _loadingMeshChunks.Remove(chunk.ChunkIndex);
        }
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
            for (int i = 0; i < chunkThreads; i++)
            {
                // Terrain loading is simply executed on a first come first serve basis.
                // Meshes will not be created until all surrounding chunks have first created their terrain.
                if (_preLoadedTerrainBuffer[i] == null)
                {
                    if (_preLoadedTerrainChunks.Count > 0)
                        _preLoadedTerrainBuffer[i] = _preLoadedTerrainChunks.Dequeue();
                }
                // Mesh loading attempts to ensure no two meshes are being worked on at the same time.
                if (_preLoadedMeshBuffer[i] == null && _preLoadedTerrainChunks.Count == 0)
                {
                    lock (meshChunkLock)
                    {
                        foreach (IChunk chunk in _preLoadedMeshChunks.Values)
                        {
                            if (!AddLoadingMeshChunk(chunk))
                                continue;
                            _preLoadedMeshBuffer[i] = chunk;
                            _preLoadedMeshChunks.Remove(chunk.ChunkIndex);
                            break;
                        }
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
                RemoveLoadingMeshChunk(chunk);
            }

        }
    }

}