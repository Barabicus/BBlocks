using UnityEngine;
using System.Collections;

public class GlassBlock : Block {

    public override byte BlockID
    {
        get { return BlockDetails.GlassBlock; }
    }

    public override Vector2 TopUV
    {
        get
        {
            return BlockDetails.glassUV;
        }
    }

}
