using UnityEngine;
using System.Collections;

public interface IBiomeGenerator
{
    void GenerateBiome(IChunk chunk);

    float GetPerlin(IntVector3 chunkPosition, int maxHeight);
}
