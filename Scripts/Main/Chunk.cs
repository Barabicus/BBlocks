using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Chunk : MonoBehaviour, IChunk
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
    private List<ITick> _tickableBlock;

    private IBlock[, ,] blocks;

    private IntVector3 _chunkPosition;
    private IntVector3 _chunkIndex;
    /// <summary>
    /// Cached position of this chunk's position. This can also be used as a key to retrieve the chunk
    /// in the world chunk list.
    /// </summary>
    public IntVector3 ChunkPosition { get { return _chunkPosition; } }
    public IntVector3 ChunkIndex { get { return _chunkIndex; } }
    private bool IsMeshDirty { get; set; }

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

    public IBlock[, ,] Blocks
    {
        get { return blocks; }
    }

    public World World
    {
        get { return world;}
    }



    public void Awake()
    {
        _tickableBlock = new List<ITick>();
        _chunkPosition = new IntVector3(transform.position);
        _chunkIndex = _chunkPosition / Chunk.chunkSize;
        blocks = new IBlock[chunkSize, chunkSize, chunkSize];
    }

    public void Start()
    {
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

    public IBlock GetBlockRelativePosition(int x, int y, int z)
    {
        if (x >= 0 && x <= Chunk.chunkSize - 1 && y >= 0 && y <= Chunk.chunkSize - 1 && z >= 0 && z <= Chunk.chunkSize - 1)
        {
            return GetBlock(x, y, z);
        }
        else
        {
            return world.GetBlockWorldCoordinate(ChunkPosition + new IntVector3(x, y, z));
        }
    }

    public IBlock GetBlockRelativePosition(IntVector3 localPos)
    {
        return GetBlockRelativePosition(localPos.x, localPos.y, localPos.z);
    }

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
            //Debug.LogError("not within range: " + new IntVector3(x,y,z));
            return null;
        }
        return blocks[x, y, z];
    }

    public IBlock GetBlock(IntVector3 vector)
    {
        return GetBlock(vector.x, vector.y, vector.z);
    }

    /// <summary>
    /// Takes in a block reference and returns its local position within the chunk. If the block cannot be found
    /// in this chunk it returns null.
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public IntVector3? BlockToLocalPosition(IBlock block)
    {
        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int z = 0; z < blocks.GetLength(2); z++)
                {
                    if (blocks[x, y, z] == block)
                        return new IntVector3(x, y, z);
                }
            }
        }
        return null;
    }

    public IntVector3? BlockToWorldPosition(IBlock block)
    {
        IntVector3? position = BlockToLocalPosition(block);
        if (position.HasValue)
        {
            return new IntVector3?(LocalPositionToWorldPosition(position.Value));
        }
        return null;
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

    public IntVector3 LocalPositionToWorldPosition(IntVector3 vector)
    {
        return LocalPositionToWorldPosition(vector.x, vector.y, vector.z);
    }

    /// <summary>
    /// Takes a block world position and returns the local position of the block within the chunk
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public IntVector3 WorldPositionToLocalPosition(IntVector3 position)
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
        if (blocks[x, y, z] != null)
            blocks[x, y, z].BlockDestroyed(this);
        blocks[x, y, z] = value;
        if (blocks[x, y, z] != null)
            blocks[x, y, z].BlockPlaced(this);
        CreateMesh();

        //If we hit a block bordering a neighbouring chunk update that chunk as well
        if (x == 0)
        {
            IChunk c = world.GetChunkWorldCoordinate(ChunkPosition.x - chunkSize, ChunkPosition.y, ChunkPosition.z);
            if (c != null)
                c.CreateMesh();
        }
        if (y == 0)
        {
            IChunk c = world.GetChunkWorldCoordinate(ChunkPosition.x, ChunkPosition.y - chunkSize, ChunkPosition.z);
            if (c != null)
                c.CreateMesh();
        }
        if (z == 0)
        {
            IChunk c = world.GetChunkWorldCoordinate(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z - chunkSize);
            if (c != null)
                c.CreateMesh();
        }

        if (x == chunkSize - 1)
        {
            IChunk c = world.GetChunkWorldCoordinate(ChunkPosition.x + chunkSize, ChunkPosition.y, ChunkPosition.z);
            if (c != null)
                c.CreateMesh();
        }
        if (y == chunkSize - 1)
        {
            IChunk c = world.GetChunkWorldCoordinate(ChunkPosition.x, ChunkPosition.y + chunkSize, ChunkPosition.z);
            if (c != null)
                c.CreateMesh();
        }
        if (z == chunkSize - 1)
        {
            IChunk c = world.GetChunkWorldCoordinate(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z + chunkSize);
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
      //  CreateMesh();
        mesh.Clear();
        mesh.vertices = meshVertices.ToArray();
        mesh.uv = uvMap.ToArray();
    //    mesh.uv2 = uvMap2.ToArray();
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

    /*
    /// <summary>
    /// Updates all neighbouring chunks without any block checking.
    /// </summary>
    private void UpdateNeighbouringChunks()
    {

        IChunk chunk = null;

        chunk = world.GetChunkWorldCoordinate(ChunkPosition.x - chunkSize, ChunkPosition.y, ChunkPosition.z);
        if (chunk != null)
            chunk.UpdateMesh();
        chunk = null;

        chunk = world.GetChunkWorldCoordinate(ChunkPosition.x, ChunkPosition.y - chunkSize, ChunkPosition.z);
        if (chunk != null)
            chunk.UpdateMesh();
        chunk = null;

        chunk = world.GetChunkWorldCoordinate(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z - chunkSize);
        if (chunk != null)
            chunk.UpdateMesh();
        chunk = null;

        chunk = world.GetChunkWorldCoordinate(ChunkPosition.x + chunkSize, ChunkPosition.y, ChunkPosition.z);
        if (chunk != null)
            chunk.UpdateMesh();
        chunk = null;

        chunk = world.GetChunkWorldCoordinate(ChunkPosition.x, ChunkPosition.y + chunkSize, ChunkPosition.z);
        if (chunk != null)
            chunk.UpdateMesh();
        chunk = null;

        chunk = world.GetChunkWorldCoordinate(ChunkPosition.x, ChunkPosition.y, ChunkPosition.z + chunkSize);
        if (chunk != null)
            chunk.UpdateMesh();

    }*/

    public void ForceCreateMesh()
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

    bool isMeshCreating = false;
    public void CreateMesh()
    {
        if (!isMeshCreating)
        {
            isMeshCreating = true;
            StartCoroutine(CreateMeshCoroutine());
        }
    }

    IEnumerator CreateMeshCoroutine()
    {
        yield return new WaitForEndOfFrame();
        ForceCreateMesh();
        isMeshCreating = false;
    }

    #endregion

    #region Chunk Updates

    public void AddTickableBlock(ITick tickable)
    {
        StartCoroutine(AddTickableBlockRoutine(tickable));
    }

    private IEnumerator AddTickableBlockRoutine(ITick tickable)
    {
        yield return new WaitForEndOfFrame();
        _tickableBlock.Add(tickable);
    }

    public void RemoveTickableBlock(ITick tickable)
    {
        StartCoroutine(RemoveTickableBlockRoutine(tickable));
    }

    private IEnumerator RemoveTickableBlockRoutine(ITick tickable)
    {
        yield return new WaitForEndOfFrame();
        _tickableBlock.Remove(tickable);
    }

    public void Tick()
    {
        foreach (ITick tickable in _tickableBlock)
        {
            tickable.Tick(this);
        }
    }

    #endregion
}
