using UnityEngine;
using System.Collections;

public interface IBiome
{

    Block GetBlock(int x, int y, int z);

    float GetPerlin(IntVector3 chunkPosition, int maxHeight);
}
