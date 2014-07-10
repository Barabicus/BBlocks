using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

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

    private float _lastTime;
    private List<IChunk> tempList = new List<IChunk>();
    private int loaded = 0;
    private int threadsRunning = 0;
    private List<IWorldAnchor> _worldAnchors;

    private Dictionary<IntVector3, IChunk> _chunks;
    private Queue<IEnumerator> _meshQueue;
    private List<Thread> _renderThreads;
    //IChunk[, ,] _chunks = null;

    bool loading;

    public Dictionary<IntVector3, IChunk> Chunks
    {
        get
        {
            return _chunks;
        }
    }

    public void Awake()
    {
        _chunks = new Dictionary<IntVector3, IChunk>();
        _meshQueue = new Queue<IEnumerator>();
        _renderThreads = new List<Thread>();
        _worldAnchors = new List<IWorldAnchor>();
        //  _chunks = new Chunk[ChunksX, ChunksY, ChunksZ];
    }

    public void Start()
    {
        maxHeight = ChunksY * Chunk.chunkSize;
        _lastTime = Time.time;
        _worldAnchors.Add(player.Find("Camera").GetComponent<Player>());
        GenerateChunks();
    }

    string loadLabel;
    void OnGUI()
    {
        if (loading)
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2, Screen.height / 2, 250, 250));
            //    GUILayout.Label("Loading: " + loaded + " / " + Chunks.Length);
            GUILayout.Label(loadLabel);
            GUILayout.EndArea();
        }
    }

    void Update()
    {
      //  UpdateChunkBounds();

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
        //IntVector3 chunkIndex = new IntVector3(bounds.center) + (new IntVector3(bounds.extents) / Chunk.chunkSize);
        //for (int x = -chunkIndex.x; x < chunkIndex.x; x++)
        //{
        //    for (int z = -chunkIndex.z; z < chunkIndex.z; z++)
        //    {
        //        for (int y = 0; y < ChunksY; y++)
        //        {
        //       //     Debug.Log(new IntVector3(x, y, z));
        //            if (!Chunks.ContainsKey(new IntVector3(x, y, z)))
        //            {
        //                IChunk ch = AddChunk(new IntVector3(x, y, z));
        //                biome.GenerateChunk(ch);
        //                ch.CreateMesh();
        //            }
        //        }
        //    }
        //}

        for (int y = 0; y < ChunksY; y++)
        {
            Vector3 pos = player.transform.position;
            pos.y = 0;
            if (!Chunks.ContainsKey(WorldCoordinateToChunkIndex(pos + new Vector3(0, y * Chunk.chunkSize,0))))
            {
                IChunk ch = AddChunk(WorldCoordinateToChunkIndex(pos + new Vector3(0, y * Chunk.chunkSize,0)));
                biome.GenerateChunk(ch);
                ch.CreateMesh();
            }
        }

 
    }

    #region Debug

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_worldAnchors == null)
            return;
        foreach (IWorldAnchor anchor in _worldAnchors)
        {
            Gizmos.DrawWireCube(anchor.AnchorBounds.center, anchor.AnchorBounds.extents * 2);
        }

    }

    #endregion

    #region Generation

    void GenerateChunks()
    {
        //for (int x = -ChunksX; x < ChunksX; x++)
        //{
        //    for (int y = 0; y < ChunksY; y++)
        //    {
        //        for (int z = -ChunksZ; z < ChunksZ; z++)
        //        {
        //            AddChunk(x, y, z);
        //        }
        //    }
        //}
        Bounds playerBounds = _worldAnchors[0].AnchorBounds;
        for (int x = (int)-(playerBounds.extents.x / Chunk.chunkSize); x < (playerBounds.extents.x / Chunk.chunkSize); x++)
        {
            for (int z = (int)-(playerBounds.extents.z / Chunk.chunkSize); z < (playerBounds.extents.z / Chunk.chunkSize); z++)
            {
                for (int y = 0; y < ChunksY; y++)
                {
                    AddChunk(new IntVector3(x, y, z));
                }
            }
        }

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
        //Chunks[x, y, z] = chunk;
        go.transform.parent = transform;
        return chunk;
    }

    IChunk AddChunk(IntVector3 index)
    {
        return AddChunk(index.x, index.y, index.z);
    }

    IEnumerator UpdateChunks()
    {
        yield return new WaitForEndOfFrame();

        loading = true;


        foreach (IChunk chunk in Chunks.Values)
        {
            tempList.Add(chunk as IChunk);
        }

        IEnumerator terrainIT = GenerateTerrainMulti(1);
        IEnumerator meshIT = GenerateMeshMulti(2);

        //IEnumerator terrainIT = GenerateTerrainSingle();
        //IEnumerator meshIT = GenerateMeshSingle();


        while (terrainIT.MoveNext())
        {
            yield return null;
        }

        while (meshIT.MoveNext())
        {
            yield return null;
        }

        loading = false;

        while (loading == true)
            yield return null;


        yield return new WaitForEndOfFrame();

        RaycastHit hit;
        if (Physics.Raycast(new Vector3(0, (ChunksY * Chunk.chunkSize) + 1, 0), -Vector3.up, out hit))
        {
            player.transform.position = hit.point + new Vector3(0, 2, 0);
        }
        else
        {
            player.transform.position = new Vector3(50, 250, 50);
            Debug.Log("Could not set pos");
        }
        player.gameObject.SetActive(true);

        loading = false;

    }

    IEnumerator GenerateTerrainMulti(int threads)
    {
        for (int i = 0; i < threads; i++)
        {
            Thread t = new Thread(() =>
            {
                IChunk ch = NextChunk(tempList);
                while (ch != null)
                {
                    biome.GenerateChunk(ch);
                    loaded++;
                    ch = NextChunk(tempList);
                    Thread.Sleep(0);
                }
                threadsRunning--;
            });
            t.Start();
            threadsRunning++;
        }

        while (threadsRunning > 0)
        {
            loadLabel = "Generating Terrain: " + loaded + " / " + Chunks.Count;
            yield return null;
        }

    }

    IEnumerator GenerateTerrainSingle()
    {
        // Create Terrain
        foreach (IChunk chunk in Chunks.Values)
        {
            biome.GenerateChunk(chunk);
            loaded++;
            loadLabel = "Generating Terrain: " + loaded + " / " + Chunks.Count;
            yield return null;
        }
        yield break;
    }

    IEnumerator GenerateMeshMulti(int threads)
    {
        tempList.Clear();

        foreach (IChunk chunk in Chunks.Values)
        {
            tempList.Add(chunk);
        }

        threadsRunning = 0;
        loaded = 0;
        for (int i = 0; i < threads; i++)
        {
            Thread t = new Thread(() =>
            {
                IChunk ch = NextChunk(tempList);
                while (ch != null)
                {
                    ch.ForceCreateMesh();
                    loaded++;
                    ch = NextChunk(tempList);
                    Thread.Sleep(5);
                }
                threadsRunning--;
            });
            t.Start();
            threadsRunning++;
        }

        while (threadsRunning > 0)
        {
            loadLabel = "Creating Meshes: " + loaded + " / " + Chunks.Count + " : " + threadsRunning;
            yield return null;
        }
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

