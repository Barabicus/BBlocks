using UnityEngine;
using System.Collections;

public class GenericBiome : MonoBehaviour, IBiomeGenerator
{

    public int GrassStartPos = 50;

    public AnimationCurve perlinStrength = AnimationCurve.Linear(0, 1, 1, 1);
    public float Octaves = 4.5f;
    public float TopDownOctaves = 6f;
    public float HeightWeight = 4.5f;

    public void GenerateBiome(IChunk chunk)
    {
        for (int x = 0; x < chunk.Blocks.GetLength(0); x++)
        {
            for (int y = 0; y < chunk.Blocks.GetLength(1); y++)
            {
                for (int z = 0; z < chunk.Blocks.GetLength(2); z++)
                {
                    IntVector3 blockPosition = chunk.LocalPositionToWorldPosition(x, y, z);
                    float Overhang = GetPerlin(blockPosition, chunk.World.maxHeight);

                    if (blockPosition.y < GrassStartPos)
                    {
                        chunk[x, y, z] = new StoneBlock();
                    }
                    else
                    {
                        if (Overhang > 0.5f)
                        {
                            if (GetPerlin(blockPosition + new IntVector3(0, 1, 0), chunk.World.maxHeight) > 0.5f)
                                chunk[x, y, z] = new DirtBlock();
                            else
                                chunk[x, y, z] = new GrassBlock();
                        }
                        else
                        {
                            chunk[x, y, z] = null;
                        }
                    }


                }
            }
        }
    }


    public float GetPerlin(IntVector3 blockPosition, int maxHeight)
    {
    //    float perlin = Mathf.PerlinNoise((blockPosition.x / (float)maxHeight) * Octaves, (blockPosition.y / (float)maxHeight) * Octaves);
        float perlin = SimplexNoise.Noise( blockPosition.x / (float)maxHeight * Octaves, blockPosition.y / (float)maxHeight * Octaves, blockPosition.z  / (float)maxHeight * Octaves);
      //  perlin += 1 * 0.5f;
        perlin = Mathf.Abs(perlin);
        perlin = Mathf.Clamp(perlin, 0f, 1f);
        perlin *= HeightMod(blockPosition, maxHeight);
        perlin *= TopDownPerlin(blockPosition, maxHeight);
        perlin *= perlinStrength.Evaluate(blockPosition.y / (float)maxHeight);
        perlin = Mathf.Round(perlin); 
        return perlin;
    }

    float TopDownPerlin(IntVector3 blockPosition, int maxHeight)
    {
        float perlin = Mathf.PerlinNoise((blockPosition.x / (float)maxHeight) * TopDownOctaves, (blockPosition.z / (float)maxHeight) * TopDownOctaves);
        perlin *= HeightMod(blockPosition, maxHeight);
        return perlin;
    }

    float HeightMod(IntVector3 blockPosition, int maxHeight)
    {
        return ((maxHeight - blockPosition.y) / (float)maxHeight) * HeightWeight;
    }

}
