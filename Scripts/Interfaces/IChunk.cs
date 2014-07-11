using UnityEngine;
using System.Collections;

public interface IChunk
{

    /// <summary>
    /// Cached position of this chunk's position. This can also be used as a key to retrieve the chunk
    /// in the world chunk list.
    /// </summary>
    IntVector3 ChunkPosition { get; }
    IntVector3 ChunkIndex { get; }

    bool IsLoaded { get; set; }

    bool NeighbouringChunksLoaded { get; }

    IChunk TopChunk { get; }
    IChunk BottomChunk { get; }
    IChunk ForwardChunk { get; }
    IChunk BehindChunk { get; }
    IChunk LeftChunk { get; }
    IChunk RightChunk { get; }

    IBlock this[int x, int y, int z] { get; set; }

    IBlock[,,] Blocks { get; }

    World World { get; }

    #region Block
    /// <summary>
    /// Get block in relative position to this chunk. I.e block 0, 0, 0 is the first block in this chunk.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    IBlock GetBlock(int x, int y, int z);

    IBlock GetBlock(IntVector3 vector);

    /// <summary>
    /// Gets the world position of the block in this chunk at chunk local position x,y,z
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    IntVector3 LocalPositionToWorldPosition(int x, int y, int z);

    /// <summary>
    /// Takes a block world position and returns the local position of the block within the chunk
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    IntVector3 WorldPositionToLocalPosition(IntVector3 position);

    /// <summary>
    /// Sets the block at local position x,y,z
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool SetBlock(int x, int y, int z, IBlock value);

    bool SetBlock(IntVector3 vector, IBlock value);

    #endregion
    void ForceCreateMesh();

    void CreateMesh();

    void Tick();

}