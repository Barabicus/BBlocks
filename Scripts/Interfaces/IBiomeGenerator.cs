using UnityEngine;
using System.Collections;

public interface IBiomeGenerator
{
    void GenerateBiome(Chunk chunk);
}
