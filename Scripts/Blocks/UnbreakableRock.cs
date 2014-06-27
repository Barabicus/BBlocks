using UnityEngine;
using System.Collections;

public class UnbreakableRock : Block
{
    public override byte BlockID
    {
        get { return BlockDetails.UnbreakableBlock; }
    }

    public override Vector2 TopUV
    {
        get
        {
            return BlockDetails.unbreakableUV;
        }
    }
}
