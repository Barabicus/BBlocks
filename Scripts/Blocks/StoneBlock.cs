using UnityEngine;
using System.Collections;

public class StoneBlock : Block
{

    public override byte BlockID
    {
        get { return BlockDetails.StoneBlock; }
    }

    public override Vector2 TopUV
    {
        get
        {
            return BlockDetails.stoneUV;
        }
    }
}
