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
        IntVector3? localPos = chunk.BlockToLocalPosition(this);
        if (localPos.HasValue)
        {
            IntVector3 blockPos = chunk.LocalPositionToWorldPosition(localPos.Value);
            chunk.world.SetBlockWorldCoordinate(blockPos + new IntVector3(0, 1, 0), new VirusBlock());
            chunk.world.SetBlockWorldCoordinate(blockPos + new IntVector3(0, -1, 0), new VirusBlock());
            chunk.world.SetBlockWorldCoordinate(blockPos + new IntVector3(-1, 0, 0), new VirusBlock());
            chunk.world.SetBlockWorldCoordinate(blockPos + new IntVector3(1, 0, 0), new VirusBlock());
            chunk.world.SetBlockWorldCoordinate(blockPos + new IntVector3(0, 0, 1), new VirusBlock());
            chunk.world.SetBlockWorldCoordinate(blockPos + new IntVector3(0, 0, -1), new VirusBlock());

            chunk.SetBlock(localPos.Value, new StoneBlock());
        }
    }
}
