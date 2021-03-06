﻿using UnityEngine;
using System.Collections;

public class GrassBlock : Block {

    public override Vector2 TopUV
    {
        get
        {
            return BlockDetails.grassUV;
        }
    }

    public override Vector2 NorthUV
    {
        get
        {
            return BlockDetails.grassSideUV;
        }
    }

    public override Vector2 SouthUV
    {
        get
        {
            return BlockDetails.grassSideUV;
        }
    }

    public override Vector2 EastUV
    {
        get
        {
            return BlockDetails.grassSideUV;
        }
    }

    public override Vector2 WestUV
    {
        get
        {
            return BlockDetails.grassSideUV;
        }
    }

    public override Vector2 BottomUV
    {
        get
        {
            return BlockDetails.dirtUV;
        }
    }

    public override byte BlockID
    {
        get { return BlockDetails.GrassBlock; }
    }
}
