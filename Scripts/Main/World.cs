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

    public Texture2D textureAtlas;
    public Material ChunkMaterial;
    public BiomeController biome;
    public int maxHeight;

    //  Dictionary<IntVector3, Chunk> _chunks;
    Chunk[, ,] _chunks;

    public Chunk[, ,] Chunks
    {
        get
        {
            return _chunks;
        }
    }

    [ContextMenu("Execute")]
    void Start()
    {
        //   _chunks = new Dictionary<IntVector3, Chunk>();
        _chunks = new Chunk[ChunksX, ChunksY, ChunksZ];
        maxHeight = ChunksY * Chunk.chunkSize;
        GenerateChunks();
    }

    void GenerateChunks()
    {
        for (int x = 0; x < ChunksX; x++)
        {
            for (int y = 0; y < ChunksY; y++)
            {
                for (int z = 0; z < ChunksZ; z++)
                {
                    GameObject go = new GameObject("Chunk: (" + x + "," + y + "," + z + ")");
                    go.layer = 8;
                    go.transform.position = new Vector3(Chunk.chunkSize * x, Chunk.chunkSize * y, Chunk.chunkSize * z);
                    Chunk chunk = go.AddComponent<Chunk>();
                    go.AddComponent<MeshFilter>();
                    go.AddComponent<MeshRenderer>();
                    go.AddComponent<MeshCollider>();
                    chunk.world = this;
                    // Chunks.Add(new IntVector3(x, y, z), chunk);
                    Chunks[x, y, z] = chunk;
                    go.transform.parent = transform;
                }
            }
        }
        StartCoroutine(UpdateChunks());
    }

    IEnumerator UpdateChunks()
    {
        yield return new WaitForEndOfFrame();
        // Create Terrain
        foreach (Chunk chunk in Chunks)
        {
            biome.GetBiome().GenerateBiome(chunk);
            // yield return null;
        }
        int mod = 0;
        foreach (Chunk c in Chunks)
        {
            c.CreateMesh();
            mod++;
            if (mod % 5 == 0)
                yield return null;
        }
    }

    #region Block & Chunk Methods
    public bool SetBlockWorldCoordinate(int x, int y, int z, IBlock block)
    {
        Chunk chunk = WorldCoordinateToChunk(x, y, z);
        if (chunk == null)
            return false;
        IntVector3 pos = new IntVector3(x, y, z) - new IntVector3(chunk.transform.position);
        return chunk.SetBlock(pos, block);
    }

    public bool SetBlockWorldCoordinate(IntVector3 vector, IBlock block)
    {
        return SetBlockWorldCoordinate(vector.x, vector.y, vector.z, block);
    }

    public IBlock GetBlockWorldCoordinate(int x, int y, int z)
    {
        Chunk chunk = WorldCoordinateToChunk(x, y, z);
        if (chunk == null)
            return null;
        // Convert the world coordinate (x,y,z) into the chunks local space and get the block
        return chunk.GetBlock(new IntVector3(x, y, z) - chunk.ChunkPosition);
    }

    public IBlock GetBlockWorldCoordinate(IntVector3 vector)
    {
        return GetBlockWorldCoordinate(vector.x, vector.y, vector.z);
    }

    public Chunk WorldCoordinateToChunk(IntVector3 position)
    {
        return WorldCoordinateToChunk(position.x, position.y, position.z);
    }

    public Chunk WorldCoordinateToChunk(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= Chunks.GetLength(0) * Chunk.chunkSize || y >= Chunks.GetLength(1) * Chunk.chunkSize || z >= Chunks.GetLength(2) * Chunk.chunkSize)
            return null;

        return Chunks[x / Chunk.chunkSize, y / Chunk.chunkSize, z / Chunk.chunkSize];
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

   /* public Chunk GetChunk(int x, int y, int z)
    {
        try
        {
            return Chunks[x / Chunk.chunkSize, y / Chunk.chunkSize, z / Chunk.chunkSize];
        }
        catch (Exception e)
        {
            // TODO REMOVE EXCEPTION CATCHING IN FAVOR OF IF TESTING
            return null;
        }
    }*/

    #endregion
}

