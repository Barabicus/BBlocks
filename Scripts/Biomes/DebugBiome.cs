using UnityEngine;
using System.Collections;

public class DebugBiome : MonoBehaviour, IBiome
{

    public float GetPerlin(IntVector3 chunkPosition, int maxHeight)
    {
        return 1;
    }

    public Block GetBlock(int x, int y, int z)
    {
        return new StoneBlock();
    }
}
