﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Block : IBlock
{
    protected byte blockData = 0;

    public abstract byte BlockID { get; }

    /// <summary>
    /// Top UV returns the null texture by default. To return a different texture overload this method
    /// and return the UV vector that you wish to display. By default every other face is setup to return
    /// the top UV unless overloaded.
    /// </summary>
    public virtual Vector2 TopUV { get { return BlockDetails.nullUV; } }
    /// <summary>
    /// Return the Top UV by default unless overloaded
    /// </summary>
    public virtual Vector2 NorthUV { get { return TopUV; } }
    /// <summary>
    /// Return the Top UV by default unless overloaded
    /// </summary>
    public virtual Vector2 SouthUV { get { return TopUV; } }
    /// <summary>
    /// Return the Top UV by default unless overloaded
    /// </summary>

    public virtual Vector2 WestUV { get { return TopUV; } }
    /// <summary>
    /// Return the Top UV by default unless overloaded
    /// </summary>

    public virtual Vector2 EastUV { get { return TopUV; } }
    /// <summary>
    /// Return the Top UV by default unless overloaded
    /// </summary>

    public virtual Vector2 BottomUV { get { return TopUV; } }

    /// <summary>
    /// Is this block transparent, false by default
    /// </summary>
    public bool IsOpaque
    {
        get { return false; }
    }

    public void ConstructBlock(int x, int y, int z, Chunk chunk)
    {
        if (!(chunk.TopChunk == null && y == Chunk.chunkSize - 1) && chunk.GetBlockRelativePosition(x, y + 1, z) == null)
        {
            //Block above is air
            CreateTopFace(x, y, z, chunk);
        }

        if (!(chunk.BottomChunk == null && y == 0) && chunk.GetBlockRelativePosition(x, y - 1, z) == null)
        {
            //Block below is air
            CreateBottomFace(x, y, z, chunk);

        }

        if (!(chunk.RightChunk == null && x == Chunk.chunkSize - 1) && chunk.GetBlockRelativePosition(x + 1, y, z) == null)
        {
            //Block east is air
            CreateRightFace(x, y, z, chunk);

        }

        if (!(chunk.LeftChunk == null && x == 0) && chunk.GetBlockRelativePosition(x - 1, y, z) == null)
        {
            //Block west is air
            CreateLeftFace(x, y, z, chunk);

        }

        if (!(chunk.ForwardChunk == null && z == Chunk.chunkSize - 1) && chunk.GetBlockRelativePosition(x, y, z + 1) == null)
        {
            //Block north is air
            CreateForwardFace(x, y, z, chunk);

        }

        if (!(chunk.BehindChunk == null && z == 0) && chunk.GetBlockRelativePosition(x, y, z - 1) == null)
        {
            //Block south is air
            CreateBehindFace(x, y, z, chunk);

        }
    }
    protected void CreateTopFace(int x, int y, int z, Chunk chunk)
    {
        chunk.meshVertices.Add(new Vector3(x, y, z + 1));
        chunk.meshVertices.Add(new Vector3(x + 1, y, z + 1));
        chunk.meshVertices.Add(new Vector3(x + 1, y, z));
        chunk.meshVertices.Add(new Vector3(x, y, z));

        chunk.colVertices.Add(new Vector3(x, y, z + 1));
        chunk.colVertices.Add(new Vector3(x + 1, y, z + 1));
        chunk.colVertices.Add(new Vector3(x + 1, y, z));
        chunk.colVertices.Add(new Vector3(x, y, z));

        AddMeshTriangles(chunk);
        AddCollisionTriangles(chunk);
        AddUV(TopUV, chunk);
    }
    protected void CreateBottomFace(int x, int y, int z, Chunk chunk)
    {
        chunk.meshVertices.Add(new Vector3(x, y - 1, z));
        chunk.meshVertices.Add(new Vector3(x + 1, y - 1, z));
        chunk.meshVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        chunk.meshVertices.Add(new Vector3(x, y - 1, z + 1));

        chunk.colVertices.Add(new Vector3(x, y - 1, z));
        chunk.colVertices.Add(new Vector3(x + 1, y - 1, z));
        chunk.colVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        chunk.colVertices.Add(new Vector3(x, y - 1, z + 1));

        AddMeshTriangles(chunk);
        AddCollisionTriangles(chunk);
        AddUV(BottomUV, chunk);

    }
    protected void CreateForwardFace(int x, int y, int z, Chunk chunk)
    {
        chunk.meshVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        chunk.meshVertices.Add(new Vector3(x + 1, y, z + 1));
        chunk.meshVertices.Add(new Vector3(x, y, z + 1));
        chunk.meshVertices.Add(new Vector3(x, y - 1, z + 1));

        chunk.colVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        chunk.colVertices.Add(new Vector3(x + 1, y, z + 1));
        chunk.colVertices.Add(new Vector3(x, y, z + 1));
        chunk.colVertices.Add(new Vector3(x, y - 1, z + 1));

        AddMeshTriangles(chunk);
        AddCollisionTriangles(chunk);
        AddUV(NorthUV, chunk);

    }
    protected void CreateBehindFace(int x, int y, int z, Chunk chunk)
    {
        chunk.meshVertices.Add(new Vector3(x, y - 1, z));
        chunk.meshVertices.Add(new Vector3(x, y, z));
        chunk.meshVertices.Add(new Vector3(x + 1, y, z));
        chunk.meshVertices.Add(new Vector3(x + 1, y - 1, z));

        chunk.colVertices.Add(new Vector3(x, y - 1, z));
        chunk.colVertices.Add(new Vector3(x, y, z));
        chunk.colVertices.Add(new Vector3(x + 1, y, z));
        chunk.colVertices.Add(new Vector3(x + 1, y - 1, z));

        AddMeshTriangles(chunk);
        AddCollisionTriangles(chunk);
        AddUV(SouthUV, chunk);

    }
    protected void CreateLeftFace(int x, int y, int z, Chunk chunk)
    {
        chunk.meshVertices.Add(new Vector3(x, y - 1, z + 1));
        chunk.meshVertices.Add(new Vector3(x, y, z + 1));
        chunk.meshVertices.Add(new Vector3(x, y, z));
        chunk.meshVertices.Add(new Vector3(x, y - 1, z));

        chunk.colVertices.Add(new Vector3(x, y - 1, z + 1));
        chunk.colVertices.Add(new Vector3(x, y, z + 1));
        chunk.colVertices.Add(new Vector3(x, y, z));
        chunk.colVertices.Add(new Vector3(x, y - 1, z));

        AddMeshTriangles(chunk);
        AddCollisionTriangles(chunk);
        AddUV(WestUV, chunk);

    }
    protected void CreateRightFace(int x, int y, int z, Chunk chunk)
    {
        chunk.meshVertices.Add(new Vector3(x + 1, y - 1, z));
        chunk.meshVertices.Add(new Vector3(x + 1, y, z));
        chunk.meshVertices.Add(new Vector3(x + 1, y, z + 1));
        chunk.meshVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        chunk.colVertices.Add(new Vector3(x + 1, y - 1, z));
        chunk.colVertices.Add(new Vector3(x + 1, y, z));
        chunk.colVertices.Add(new Vector3(x + 1, y, z + 1));
        chunk.colVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        AddMeshTriangles(chunk);
        AddCollisionTriangles(chunk);
        AddUV(EastUV, chunk);
    }

    protected void AddMeshTriangles(Chunk chunk)
    {
        chunk.meshTriangles.Add(chunk.meshFaceCount * 4);
        chunk.meshTriangles.Add(chunk.meshFaceCount * 4 + 1);
        chunk.meshTriangles.Add(chunk.meshFaceCount * 4 + 2);
        chunk.meshTriangles.Add(chunk.meshFaceCount * 4);
        chunk.meshTriangles.Add(chunk.meshFaceCount * 4 + 2);
        chunk.meshTriangles.Add(chunk.meshFaceCount * 4 + 3);
        chunk.meshFaceCount++;
    }

    protected void AddCollisionTriangles(Chunk chunk)
    {
        chunk.colTriangles.Add(chunk.collisionFaceCount * 4);
        chunk.colTriangles.Add(chunk.collisionFaceCount * 4 + 1);
        chunk.colTriangles.Add(chunk.collisionFaceCount * 4 + 2);
        chunk.colTriangles.Add(chunk.collisionFaceCount * 4);
        chunk.colTriangles.Add(chunk.collisionFaceCount * 4 + 2);
        chunk.colTriangles.Add(chunk.collisionFaceCount * 4 + 3);
        chunk.collisionFaceCount++;
    }

    protected void AddUV(Vector2 texturePos, Chunk chunk)
    {
        chunk.uvMap.Add(new Vector2(BlockDetails.tUnit * texturePos.x + BlockDetails.tUnit, BlockDetails.tUnit * texturePos.y));
        chunk.uvMap.Add(new Vector2(BlockDetails.tUnit * texturePos.x + BlockDetails.tUnit, BlockDetails.tUnit * texturePos.y + BlockDetails.tUnit));
        chunk.uvMap.Add(new Vector2(BlockDetails.tUnit * texturePos.x, BlockDetails.tUnit * texturePos.y + BlockDetails.tUnit));
        chunk.uvMap.Add(new Vector2(BlockDetails.tUnit * texturePos.x, BlockDetails.tUnit * texturePos.y));

        /*
        chunk.uvMap2.Add(new Vector2(BlockDetails.tUnit * BlockDetails.unbreakableUV.x + BlockDetails.tUnit, BlockDetails.tUnit * BlockDetails.unbreakableUV.y));
        chunk.uvMap2.Add(new Vector2(BlockDetails.tUnit * BlockDetails.unbreakableUV.x + BlockDetails.tUnit, BlockDetails.tUnit * BlockDetails.unbreakableUV.y + BlockDetails.tUnit));
        chunk.uvMap2.Add(new Vector2(BlockDetails.tUnit * BlockDetails.unbreakableUV.x, BlockDetails.tUnit * BlockDetails.unbreakableUV.y + BlockDetails.tUnit));
        chunk.uvMap2.Add(new Vector2(BlockDetails.tUnit * BlockDetails.unbreakableUV.x, BlockDetails.tUnit * BlockDetails.unbreakableUV.y));
         */
    }


    public void BlockDestroyed(Chunk chunk)
    {
        if (this is ITick)
            chunk.RemoveTickableBlock(this as ITick);
    }

    public void BlockPlaced(Chunk chunk)
    {
        if (this is ITick)
            chunk.AddTickableBlock(this as ITick);
    }
}
