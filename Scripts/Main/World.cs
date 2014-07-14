using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Linq;

/// <summary>
/// The game world
/// </summary>
public class World : MonoBehaviour
{

    public int ChunksX = 16, ChunksY = 6, ChunksZ = 16;
    public Transform player;
    public Texture2D textureAtlas;
    public Material ChunkMaterial;
    public BiomeController biome;
    public int maxHeight;
    public float TickFrequency = 5.0f;
    public int ChunkThreads = 1;

    private float _lastTime;
    private List<IChunk> tempList = new List<IChunk>();
    private int loaded = 0;
    private int threadsRunning = 0;
    private List<IWorldAnchor> _worldAnchors;
    private IntVector3 _lastPositionIndex;
    private Dictionary<IntVector3, IChunk> _chunks;
    private WorldGenerator _worldGenerator;
    private IntVector3 _worldEdges;

    //IChunk[, ,] _chunks = null;

    bool loading = true;

    public int ChunkCount
    {
        get { return _chunks.Count; }
    }

    public Dictionary<IntVector3, IChunk> Chunks
    {
        get
        {
            return _chunks;
        }
    }

    public WorldGenerator WorldGenerator
    {
        get { return _worldGenerator; }
    }

    public void Awake()
    {
        _chunks = new Dictionary<IntVector3, IChunk>();
        _worldAnchors = new List<IWorldAnchor>();
        //  _chunks = new Chunk[ChunksX, ChunksY, ChunksZ];
    }

    public void Start()
    {
        maxHeight = ChunksY * Chunk.chunkSize;
        _lastTime = Time.time;
        _worldAnchors.Add(player.GetComponent<Player>());
        _worldGenerator = new WorldGenerator(this, ChunkThreads);
        GenerateChunks();
    }

    string loadLabel;
    void OnGUI()
    {
        if (loading)
        {
            GUILayout.BeginArea(new Rect((Screen.width / 2) - 100, Screen.height / 2, 250, 250));
            GUILayout.Box(loadLabel);

            GUILayout.EndArea();
        }
    }

    void OnApplicationQuit()
    {
        _worldGenerator.Running = false;
    }


    public int posindexX = 0;
    public int posindexZ = 0;

    void Update()
    {
        if (loading)
            player.transform.position = new Vector3(0, 250, 0);

        IntVector3 currentPositionIndex = WorldCoordinateToChunkIndex(player.transform.position);
        if ( Mathf.Abs(_lastPositionIndex.x - currentPositionIndex.x) >= 5 || Mathf.Abs(_lastPositionIndex.z - currentPositionIndex.z) >= 5)
            UpdateChunkBounds();

        if (Time.time - _lastTime > TickFrequency)
        {
            _lastTime = Time.time;
            foreach (IChunk chunk in Chunks.Values)
            {
                chunk.Tick();
            }
        }
    }

    void UpdateChunkBounds()
    {
        IntVector3 playerBounds = _worldAnchors[0].AnchorBounds;
        IntVector3 playerPosition = _worldAnchors[0].AnchorPosition / Chunk.chunkSize;
        _lastPositionIndex = WorldCoordinateToChunkIndex(player.transform.position);

        for (int x = -playerBounds.x / 2; x < playerBounds.x / 2; x++)
        {
            for (int y = 0; y < playerBounds.y; y++)
            {
                for (int z = -playerBounds.z / 2; z < playerBounds.z / 2; z++)
                {
                    if (!Chunks.ContainsKey(new IntVector3(x + playerPosition.x, y, z + playerPosition.z)))
                        AddChunk(x + playerPosition.x, y, z + playerPosition.z);
                }
            }
        }

        //for (int x = 0; x < playerBounds.x / 2; x++)
        //{
        //    for (int y = 0; y < playerBounds.y; y++)
        //    {
        //        for (int z = 0; z < playerBounds.z / 2; z++)
        //        {
        //            if (!Chunks.ContainsKey(new IntVector3(x + playerPosition.x, y, z + playerPosition.z)))
        //                AddChunk(x + playerPosition.x, y, z + playerPosition.z);

        //            if (x != 0 && z != 0)
        //            {
        //                if (!Chunks.ContainsKey(new IntVector3(-x + playerPosition.x, y, z + playerPosition.z)))
        //                    AddChunk(-x + playerPosition.x, y, z + playerPosition.z); ;

        //                if (!Chunks.ContainsKey(new IntVector3(x + playerPosition.x, y, -z + playerPosition.z)))
        //                    AddChunk(x + playerPosition.x, y, -z + playerPosition.z);


        //                if (!Chunks.ContainsKey(new IntVector3(-x + playerPosition.x, y, -z + playerPosition.z)))
        //                    AddChunk(-x + playerPosition.x, y, -z + playerPosition.z);
        //            }
        //        }
        //    }
        //}

        if (!loading)
        {
            Bounds bounds = new Bounds(player.transform.position, _worldAnchors[0].AnchorBounds.ToVector3() * Chunk.chunkSize);

            var tempArr = Chunks.ToArray();
            foreach (var kvp in tempArr)
            {
                if (!bounds.Contains(kvp.Value.ChunkPosition.ToVector3()))
                {
                    RemoveChunk(kvp.Key);
                }
            }
        }



        //int xIndex = 0;
        //int zIndex = 0;
        //int maxIndex = Mathf.Max(playerBounds.x, playerBounds.z);
        //for (int i = 0; i < maxIndex; i++, xIndex = Mathf.Min(xIndex + 1, (int)playerBounds.x), zIndex = Mathf.Min(zIndex + 1, (int)playerBounds.z))
        //{
        //    for (int y = 0; y < ChunksY; y++)
        //    {
        //        if (zIndex == 0 && xIndex == 0 && !Chunks.ContainsKey(new IntVector3(0, y, 0)))
        //            AddChunk(new IntVector3(0, y, 0));
        //        else
        //        {
        //            for (int z = -zIndex; z < zIndex + 1; z++)
        //            {
        //                if (!Chunks.ContainsKey(new IntVector3(_lastPositionIndex.x + xIndex, y, z + _lastPositionIndex.z)))
        //                    AddChunk(new IntVector3(xIndex + _lastPositionIndex.x, y, z + _lastPositionIndex.z));
        //                if (!Chunks.ContainsKey(new IntVector3(-xIndex + _lastPositionIndex.x, y, z + _lastPositionIndex.z)))
        //                    AddChunk(new IntVector3(-xIndex + _lastPositionIndex.x, y, z + _lastPositionIndex.z));
        //            }

        //            for (int x = -xIndex + 1; x < xIndex; x++)
        //            {
        //                if (!Chunks.ContainsKey(new IntVector3(x + _lastPositionIndex.x, y, zIndex + _lastPositionIndex.z)))
        //                    AddChunk(new IntVector3(x + _lastPositionIndex.x, y, zIndex + _lastPositionIndex.z));
        //                if (!Chunks.ContainsKey(new IntVector3(x + _lastPositionIndex.x, y, -zIndex + _lastPositionIndex.z)))
        //                    AddChunk(new IntVector3(x + _lastPositionIndex.x, y, -zIndex + _lastPositionIndex.z));
        //            }
        //        }
        //    }
        //}

    }


    #region Debug

    void OnDrawGizmos()
    {
        //if (player.transform != null)
        //{
        //    Gizmos.color = Color.green;
        //    if (posindexX == 0)
        //        Gizmos.DrawWireCube(new Vector3(0, 0, 0), new Vector3(16, 16, 16));
        //    else
        //    {
        //        Gizmos.color = Color.green;
        //        for (int z = -posindexZ; z < posindexZ + 1; z++)
        //        {
        //            Gizmos.DrawWireCube(new Vector3((posindexX * Chunk.chunkSize) + player.transform.position.x, 0, z * Chunk.chunkSize + player.transform.position.z), new Vector3(Chunk.chunkSize, Chunk.chunkSize, Chunk.chunkSize));
        //            Gizmos.DrawWireCube(new Vector3(-posindexX * Chunk.chunkSize + player.transform.position.x, 0, z * Chunk.chunkSize + player.transform.position.z), new Vector3(Chunk.chunkSize, Chunk.chunkSize, Chunk.chunkSize));
        //        }

        //        Gizmos.color = Color.magenta;
        //        for (int x = -posindexX + 1; x < posindexX; x++)
        //        {
        //            Gizmos.DrawWireCube(new Vector3(x * Chunk.chunkSize + player.transform.position.x, 0, posindexZ * Chunk.chunkSize + player.transform.position.z), new Vector3(Chunk.chunkSize, Chunk.chunkSize, Chunk.chunkSize));
        //            Gizmos.DrawWireCube(new Vector3(x * Chunk.chunkSize + player.transform.position.x, 0, -posindexZ * Chunk.chunkSize + player.transform.position.z), new Vector3(Chunk.chunkSize, Chunk.chunkSize, Chunk.chunkSize));
        //        }

        //    }
        //}

        Gizmos.color = Color.red;
        if (_worldAnchors == null)
            return;
        foreach (IWorldAnchor anchor in _worldAnchors)
        {
            Gizmos.DrawWireCube(anchor.AnchorPosition.ToVector3(), anchor.AnchorBounds.ToVector3() * Chunk.chunkSize);
        }



    }

    #endregion

    #region Generation

    void GenerateChunks()
    {
        UpdateChunkBounds();

        StartCoroutine(_worldGenerator.ChunkLoader());
        StartCoroutine(UpdateChunks());
    }

    IChunk AddChunk(int x, int y, int z)
    {
        GameObject go = new GameObject("Chunk: (" + x + "," + y + "," + z + ")");
        go.layer = 8;
        go.transform.position = new Vector3(Chunk.chunkSize * x, Chunk.chunkSize * y, Chunk.chunkSize * z);
        Chunk chunk = go.AddComponent<Chunk>();
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshCollider>();
        chunk.world = this;
        Chunks.Add(new IntVector3(x, y, z), chunk);
        _worldGenerator.GenerateChunk(chunk);
        //Chunks[x, y, z] = chunk;
        go.transform.parent = transform;
        return chunk;
    }

    IChunk AddChunk(IntVector3 index)
    {
        return AddChunk(index.x, index.y, index.z);
    }

    void RemoveChunk(int x, int y, int z)
    {
        IChunk chunk;
        if (Chunks.TryGetValue(new IntVector3(x, y, z), out chunk))
        {
            Chunks.Remove(chunk.ChunkIndex);
            Destroy((chunk as MonoBehaviour).gameObject);
        }
    }

    void RemoveChunk(IntVector3 index)
    {
        RemoveChunk(index.x, index.y, index.z);
    }

    IEnumerator UpdateChunks()
    {
        yield return new WaitForEndOfFrame();

        StartCoroutine(PlacePlayer());
    }

    private IEnumerator PlacePlayer()
    {
        while (!Physics.Raycast(new Vector3(0, (ChunksY * Chunk.chunkSize) + 1, 0), -Vector3.up))
        {
            loadLabel = "Left to load: " + _worldGenerator.PreLoadedTerrainCount;
            yield return null;
        }
        loading = false;

        RaycastHit hit;
        if (Physics.Raycast(new Vector3(0, (ChunksY * Chunk.chunkSize) + 1, 0), -Vector3.up, out hit))
        {
            player.transform.position = hit.point + new Vector3(0, 2, 0);
        }
        else
        {
            player.transform.position = new Vector3(10, 350, 10);
            Debug.Log("Could not set pos");
        }
        player.gameObject.SetActive(true);
        _lastPositionIndex = WorldCoordinateToChunkIndex(player.transform.position);
    }

    #endregion

    #region Block & Chunk Methods


    private static readonly object syncLock = new object();
    public T NextChunk<T>(List<T> ch)
    {
        lock (syncLock)
        {
            if (ch.Count == 0)
                return default(T);
            T c = ch[0];
            ch.RemoveAt(0);
            return c;
        }
    }

    public IntVector3 WorldCoordinateToChunkIndex(Vector3 position)
    {
        return new IntVector3(position.x / Chunk.chunkSize, position.y / Chunk.chunkSize, position.z / Chunk.chunkSize);
    }

    public bool SetBlockWorldCoordinate(int x, int y, int z, IBlock block)
    {
        IChunk chunk = GetChunkWorldCoordinate(x, y, z);
        if (chunk == null)
            return false;
        IntVector3 pos = new IntVector3(x, y, z) - chunk.ChunkPosition;
        return chunk.SetBlock(pos, block);
    }

    public bool SetBlockWorldCoordinate(IntVector3 vector, IBlock block)
    {
        return SetBlockWorldCoordinate(vector.x, vector.y, vector.z, block);
    }

    public IBlock GetBlockWorldCoordinate(int x, int y, int z)
    {
        IChunk chunk = GetChunkWorldCoordinate(x, y, z);
        if (chunk == null)
            return null;
        // Convert the world coordinate (x,y,z) into the chunks local space and get the block
        return chunk.GetBlock(new IntVector3(x, y, z) - chunk.ChunkPosition);
    }

    public IBlock GetBlockWorldCoordinate(IntVector3 position)
    {
        return GetBlockWorldCoordinate(position.x, position.y, position.z);
    }

    public IBlock GetBlockWorldCoordinate(Vector3 position)
    {
        return GetBlockWorldCoordinate(new IntVector3(position));
    }

    /// <summary>
    /// Takes a world coordinate and converts it into the chunk that contains that position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>Return the chunk if found at the specified world coordinate otherwise it returns null</returns>
    public IChunk GetChunkWorldCoordinate(IntVector3 position)
    {
        return GetChunkWorldCoordinate(position.x, position.y, position.z);
    }

    public IChunk GetChunkWorldCoordinate(Vector3 position)
    {
        return GetChunkWorldCoordinate(new IntVector3(position));
    }

    /// <summary>
    /// Takes a world coordinate and converts it into the chunk that contains that position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>Return the chunk if found at the specified world coordinate otherwise it returns null</returns>
    public IChunk GetChunkWorldCoordinate(int x, int y, int z)
    {
        //    if (x < 0 || y < 0 || z < 0 || x >= Chunks.GetLength(0) * Chunk.chunkSize || y >= Chunks.GetLength(1) * Chunk.chunkSize || z >= Chunks.GetLength(2) * Chunk.chunkSize)
        //        return null;

        //   return Chunks[x / Chunk.chunkSize, y / Chunk.chunkSize, z / Chunk.chunkSize];
        IChunk chunk;
        x = Mathf.FloorToInt((float)x / Chunk.chunkSize);
        y = Mathf.FloorToInt((float)y / Chunk.chunkSize);
        z = Mathf.FloorToInt((float)z / Chunk.chunkSize);



        if (Chunks.TryGetValue(new IntVector3(x, y, z), out chunk))
        {
            return chunk;
        }
        return null;
    }

    /// <summary>
    /// Gets the chunk at the specified Index.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>Returns the chunk found at the indices suppled otherwise returns null if out of bounds of the array</returns>
    public IChunk GetChunk(int x, int y, int z)
    {
        //  if (x < 0 || y < 0 || z < 0 || x >= Chunks.GetLength(0) || y >= Chunks.GetLength(1) || z >= Chunks.GetLength(2))
        //      return null;
        //   return Chunks[x, y, z];
        IChunk chunk;
        if (Chunks.TryGetValue(new IntVector3(x, y, z), out chunk))
        {
            return chunk;
        }
        return null;
    }

    /// <summary>
    /// Gets the chunk at the specified index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IChunk GetChunk(IntVector3 index)
    {
        return GetChunk(index.x, index.y, index.z);
    }

    /// <summary>
    /// Convert a raycast hit into a world block position.
    /// </summary>
    /// <param name="hit">The raycast hit</param>
    /// <param name="vector3Pos">The original Vector 3 Position</param>
    /// <returns></returns>
    public IntVector3 RaycastHitToBlock(RaycastHit hit, out Vector3 vector3Pos)
    {
        // Move the hit of the raycast further into the block by using the normal times minus half a block length
        Vector3 aHit = hit.point + hit.normal * -0.5f;
        vector3Pos = new Vector3(Mathf.Floor(aHit.x), Mathf.Ceil(aHit.y), Mathf.Floor(aHit.z));
        return new IntVector3(vector3Pos);
    }

    public IntVector3 RaycastHitToFace(RaycastHit hit)
    {
        // Move the hit of the raycast further into the block by using the normal times minus half a block length
        Vector3 aHit = hit.point + hit.normal * 0.5f;
        Vector3 vector3Pos = new Vector3(Mathf.Floor(aHit.x), Mathf.Ceil(aHit.y), Mathf.Floor(aHit.z));
        return new IntVector3(vector3Pos);
    }

    public IntVector3 RaycastHitToBlock(RaycastHit hit)
    {
        Vector3 vec;
        return RaycastHitToBlock(hit, out vec);
    }

    #endregion
}

