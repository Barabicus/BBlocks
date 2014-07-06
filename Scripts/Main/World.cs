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

    Bounds bounds;

    Dictionary<IntVector3, IChunk> _chunks;
    //IChunk[, ,] _chunks = null;

    bool loading;

    public Dictionary<IntVector3, IChunk> Chunks
    {
        get
        {
            return _chunks;
        }
    }

    void Awake()
    {
        _chunks = new Dictionary<IntVector3, IChunk>();
        //  _chunks = new Chunk[ChunksX, ChunksY, ChunksZ];
    }

    void Start()
    {
        maxHeight = ChunksY * Chunk.chunkSize;
        _lastTime = Time.time;
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
        bounds = new Bounds(player.transform.position, new Vector3(50, 50, 50));

        //foreach (Transform t in transform)
        //{
        //    if (!bounds.Contains(t.transform.position))
        //    {
        //        t.gameObject.SetActive(false);
        //    }
        //    else
        //        t.gameObject.SetActive(true);
        //}

        if (Time.time - _lastTime > TickFrequency)
        {
            _lastTime = Time.time;
            foreach (IChunk chunk in Chunks.Values)
            {
                chunk.Tick();
            }
        }
    }

    #region Debug

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }

    #endregion

    #region Generation

    void GenerateChunks()
    {
        for (int x = 0; x < ChunksX; x++)
        {
            for (int y = 0; y < ChunksY; y++)
            {
                for (int z = 0; z < ChunksZ; z++)
                {
                    AddChunk(x, y, z);
                }
            }
        }
        StartCoroutine(UpdateChunks());
    }

    void AddChunk(int x, int y, int z)
    {
        GameObject go = new GameObject("Chunk: (" + x + "," + y + "," + z + ")");
        go.isStatic = true;
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
    }

    List<IChunk> tempList = new List<IChunk>();
    int loaded = 0;

    int threadsRunning = 0;

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

        //  RaycastHit hit;
        //if (Physics.Raycast(new Vector3((Chunks.GetLength(0) * Chunk.chunkSize) / 2, 1000f, (Chunks.GetLength(2) * Chunk.chunkSize) / 2), -Vector3.up, out hit))
        //{
        //    player.transform.position = hit.point + new Vector3(0, 2, 0);
        //}
        //else
        //{
        //    player.transform.position = new Vector3(50, 250, 50);
        //    Debug.Log("Could not set pos");
        //}
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
                    biome.GetBiome(ch).GenerateBiome(ch);
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
            biome.GetBiome(chunk).GenerateBiome(chunk);
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

    IEnumerator GenerateMeshSingle()
    {

        loaded = 0;
        foreach (IChunk c in Chunks.Values)
        {
            loaded++;
            c.ForceCreateMesh();
            loadLabel = "Creating Mesh: " + loaded + " / " + Chunks.Count;
            yield return null;
        }
    }

    #endregion

    #region Block & Chunk Methods


    private readonly object syncLock = new object();
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

    public IBlock GetBlockWorldCoordinate(IntVector3 vector)
    {
        return GetBlockWorldCoordinate(vector.x, vector.y, vector.z);
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
        if (Chunks.TryGetValue(new IntVector3(x / Chunk.chunkSize, y / Chunk.chunkSize, z / Chunk.chunkSize), out chunk))
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
        return Chunks[new IntVector3(x, y, z)];
    }

    /// <summary>
    /// Convert a raycast hit into a world block position.
    /// </summary>
    /// <param name="hit">The raycast hit</param>
    /// <param name="vector3Pos">The original Vector 3 Position</param>
    /// <returns></returns>
    public IntVector3 RaycastHitToBlock(RaycastHit hit, out Vector3 vector3Pos, float offset)
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
        return RaycastHitToBlock(hit, out vec, 0f);
    }

    #endregion
}

