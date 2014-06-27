using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{

    public const int chunkSize = 16;

    public List<Vector3> meshVertices = new List<Vector3>();
    public List<int> meshTriangles = new List<int>();
    public List<Vector2> uvMap = new List<Vector2>();
    public List<Vector2> uvMap2 = new List<Vector2>();
    public List<Vector3> colVertices = new List<Vector3>();
    public List<int> colTriangles = new List<int>();

    public int meshFaceCount;
    public int collisionFaceCount;
    public World world;

    private Mesh mesh;
    private Mesh collisionMesh;
    private MeshCollider col;

    public IBlock[, ,] blocks;

    /// <summary>
    /// Cached position of this chunk's position. This can also be used as a key to retrieve the chunk
    /// in the world chunk list.
    /// </summary>
    public IntVector3 ChunkPosition { get; private set; }
    public bool IsMeshDirty { get; set; }

    public IBlock this[int x, int y, int z]
    {
        get { return blocks[x, y, z]; }
        set
        {
            // Is within range
            if (x < 0 || y < 0 || z < 0 || x > blocks.GetLength(0) - 1 || y > blocks.GetLength(1) - 1 || z > blocks.GetLength(2) - 1)
            {
                return;
            }
            blocks[x, y, z] = value;
        }
    }

    void Start()
    {
        ChunkPosition = new IntVector3(transform.position);
        blocks = new IBlock[chunkSize, chunkSize, chunkSize];
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();
        collisionMesh = new Mesh();
        renderer.material = world.ChunkMaterial;
    }

    void Update()
    {
        if (IsMeshDirty)
            UpdateMesh();
    }


    #region Block
    /// <summary>
    /// Get block in relative position to this chunk. I.e block 0, 0, 0 is the first block in this chunk.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public IBlock GetBlock(int x, int y, int z)
    {
        // Is within range
        if (x < 0 || y < 0 || z < 0 || x >= blocks.GetLength(0) || y >= blocks.GetLength(1) || z >= blocks.GetLength(2))
        {
            return null;
        }
        return blocks[x, y, z];
    }

    public IBlock GetBlock(IntVector3 vector)
    {
        return GetBlock(vector.x, vector.y, vector.z);
    }

    /// <summary>
    /// Gets the world position of the block in this chunk at chunk local position x,y,z
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public IntVector3 LocalPositionToWorldPosition(int x, int y, int z)
    {
        return new IntVector3(ChunkPosition.x + x, ChunkPosition.y + y, ChunkPosition.z + z);
    }

    /// <summary>
    /// Takes a block world position and returns the local position of the block within the chunk
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public IntVector3 WorldPositionToLocal(IntVector3 position)
    {
        return position - ChunkPosition;
    }

    /// <summary>
    /// Sets the block at local position x,y,z
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetBlock(int x, int y, int z, IBlock value)
    {
        if (blocks == null)
        {
            return false;
        }
        // Is within range
        if (x < 0 || y < 0 || z < 0 || x > blocks.GetLength(0) - 1 || y > blocks.GetLength(1) - 1 || z > blocks.GetLength(2) - 1)
        {
            return false;
        }
        blocks[x, y, z] = value;
        CreateMesh();

        //If we hit a block bordering a neighbouring chunk update that chunk as well
        if (x == 0)
        {
            Chunk c = world.WorldCoordinateToChunk(ChunkPosition.x - chunkSize, ChunkPosition.y, ChunkPosition.z);
            if (c != null)
                c.CreateMesh();
        }
        if (y == 0)
        {
            Chunk c = world.WorldCoordinateToChunk(ChunkPosition.x, ChunkPosition.y - chunkSize, ChunkPosition.z);
            if (c != null)
                c.CreateMesh();
        }
        if (z == 0)
        {
            Chunk c = world.WorldCoordinateToChunk(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z - chunkSize);
            if (c != null)
                c.CreateMesh();
        }

        if (x == chunkSize - 1)
        {
            Chunk c = world.WorldCoordinateToChunk(ChunkPosition.x + chunkSize, ChunkPosition.y, ChunkPosition.z);
            if (c != null)
                c.CreateMesh();
        }
        if (y == chunkSize - 1)
        {
            Chunk c = world.WorldCoordinateToChunk(ChunkPosition.x, ChunkPosition.y + chunkSize, ChunkPosition.z);
            if (c != null)
                c.CreateMesh();
        }
        if (z == chunkSize - 1)
        {
            Chunk c = world.WorldCoordinateToChunk(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z + chunkSize);
            if (c != null)
                c.CreateMesh();
        }

        return true;
    }

    public bool SetBlock(IntVector3 vector, IBlock value)
    {
        return SetBlock(vector.x, vector.y, vector.z, value);
    }

    #endregion

    #region Mesh & Collision Mesh


    private void UpdateMesh()
    {
       // CreateMesh();

        mesh.Clear();
        mesh.vertices = meshVertices.ToArray();
        mesh.uv = uvMap.ToArray();
        mesh.uv2 = uvMap2.ToArray();
        mesh.triangles = meshTriangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        // collisionMesh.Clear();
        Mesh colMesh = new Mesh();
        colMesh.vertices = colVertices.ToArray();
        colMesh.triangles = colTriangles.ToArray();
        col.sharedMesh = colMesh;

        colVertices.Clear();
        colTriangles.Clear();
        collisionFaceCount = 0;

        meshVertices.Clear();
        uvMap.Clear();
        uvMap2.Clear();
        meshTriangles.Clear();
        meshFaceCount = 0;

        IsMeshDirty = false;

    }

    /// <summary>
    /// Updates all neighbouring chunks without any block checking.
    /// </summary>
    private void UpdateNeighbouringChunks()
    {

        Chunk chunk = null;

        chunk = world.WorldCoordinateToChunk(ChunkPosition.x - chunkSize, ChunkPosition.y, ChunkPosition.z);
        if (chunk != null)
            chunk.CreateMesh();
        chunk = null;

        chunk = world.WorldCoordinateToChunk(ChunkPosition.x, ChunkPosition.y - chunkSize, ChunkPosition.z);
        if (chunk != null)
            chunk.CreateMesh();
        chunk = null;

        chunk = world.WorldCoordinateToChunk(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z - chunkSize);
        if (chunk != null)
            chunk.CreateMesh();
        chunk = null;

        chunk = world.WorldCoordinateToChunk(ChunkPosition.x + chunkSize, ChunkPosition.y, ChunkPosition.z);
        if (chunk != null)
            chunk.CreateMesh();
        chunk = null;

        chunk = world.WorldCoordinateToChunk(ChunkPosition.x, ChunkPosition.y + chunkSize, ChunkPosition.z);
        if (chunk != null)
            chunk.CreateMesh();
        chunk = null;

        chunk = world.WorldCoordinateToChunk(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z + chunkSize);
        if (chunk != null)
            chunk.CreateMesh();

    }

    public void CreateMesh()
    {
        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int z = 0; z < blocks.GetLength(2); z++)
                {
                    if (blocks[x, y, z] != null)
                        blocks[x, y, z].ConstructBlock(x, y, z, this);
                }
            }
        }
        IsMeshDirty = true;
    }


    #endregion

    #region Helper Methods



    #endregion


}
