using UnityEngine;
using System.Collections;

public class DirtBlock : Block {

    public override Vector2 TopUV
    {
        get
        {
            return BlockDetails.dirtUV;
        }
    }

    public override byte BlockID
    {
        get { return BlockDetails.DirtBlock; }
    }
}
