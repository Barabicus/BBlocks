using UnityEngine;
using System.Collections;

public class VirusBlock : Block, ITick
{

    public override byte BlockID
    {
        get { return BlockDetails.VirusBlock; }
    }

    public override Vector2 TopUV
    {
        get
        {
            return BlockDetails.virusUV;
        }
    }

    public void Tick(Chunk chunk)
    {
        if (blockData != 0)
            return;

        IntVector3? localPos = chunk.BlockToLocalPosition(this);
        if (localPos.HasValue)
        {
            IntVector3 blockPos = chunk.LocalPositionToWorldPosition(localPos.Value);

            SetBlock(blockPos + new IntVector3(-1, 0, 0),chunk);
            SetBlock(blockPos + new IntVector3(1, 0, 0), chunk);
            SetBlock(blockPos + new IntVector3(0, 0, 1), chunk);
            SetBlock(blockPos + new IntVector3(0, 0, -1), chunk);
            SetBlock(blockPos + new IntVector3(0, 1, 0), chunk);
            SetBlock(blockPos + new IntVector3(0, -1, 0), chunk);


            

            chunk.SetBlock(localPos.Value, new StoneBlock());

           // blockData = 1;
        }
    }

    private void SetBlock(IntVector3 position, Chunk chunk)
    {
        IBlock b = chunk.world.GetBlockWorldCoordinate(position);
        if (b == null || b.GetType() != typeof(StoneBlock))
            chunk.world.SetBlockWorldCoordinate(position, new VirusBlock());

    }

}
