using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class GrassLandsBiome : MonoBehaviour, IBiomeGenerator
{
  //  public AnimationCurve heightCurve = AnimationCurve.Linear(0, 1, 1, 1);
    public AnimationCurve perlinCurve = AnimationCurve.Linear(0, 1, 1, 1);

    public float zoom = 1f;
    public bool abs = true;

    public void GenerateBiome(Chunk chunk)
    {
        for (int x = 0; x < chunk.blocks.GetLength(0); x++)
        {
            for (int y = 0; y < chunk.blocks.GetLength(1); y++)
            {
                for (int z = 0; z < chunk.blocks.GetLength(2); z++)
                {
                    IntVector3 blockPosition = chunk.LocalPositionToWorldPosition(x, y, z);
                    float lands = GrassPerlin(blockPosition, chunk.world.maxHeight);
                    float cave = CavePerlin(blockPosition, chunk.world.maxHeight);

                    if (blockPosition.y == 0)
                    {
                        chunk[x, y, z] = new UnbreakableRock();
                        continue;
                    }

                    if (chunk.ChunkPosition.y + y < 60f)
                    {
                        if(cave > 0.085f)
                        chunk[x, y, z] = new StoneBlock();
                    }
                    else if (chunk.ChunkPosition.y + y < 128f)
                    {
                        if (lands >= (blockPosition.y) / (float)chunk.world.maxHeight)
                        {
                            if (GrassPerlin(blockPosition + new IntVector3(0, 1, 0), chunk.world.maxHeight) < (blockPosition.y + 1) / (float)chunk.world.maxHeight)
                                chunk[x, y, z] = new GrassBlock();
                            else
                                chunk[x, y, z] = new DirtBlock();
                        }
                        else
                            chunk[x, y, z] = null;
                    }


                }
            }
        }
    }

    float TunnelPerlin(IntVector3 blockPosition, int maxHeight)
    {
        float tunnel = Mathf.PerlinNoise(120 + blockPosition.x / (float)maxHeight * zoom, (blockPosition.z) / (float)maxHeight * zoom);
        tunnel -= ((blockPosition.y) / maxHeight);
        return -tunnel;
    }

    float GrassPerlin(IntVector3 blockPosition, int maxHeight)
    {
        float lands = Mathf.PerlinNoise((blockPosition.x) / (float)maxHeight * zoom, (blockPosition.z) / (float)maxHeight * zoom);
        lands *= perlinCurve.Evaluate(lands);
        lands -= ((blockPosition.y) / maxHeight);
        if (abs)
            lands += (1f * 0.5f);
        return lands;
    }

    float CavePerlin(IntVector3 blockPosition, int maxHeight)
    {
        float cave = Mathf.PerlinNoise(blockPosition.x / (float)maxHeight * zoom, blockPosition.z / (float)maxHeight * zoom);
        cave *= Mathf.PerlinNoise(50 + blockPosition.x / (float)maxHeight * zoom,  50 + blockPosition.y / (float)maxHeight * zoom);
        return cave;
    }

}
