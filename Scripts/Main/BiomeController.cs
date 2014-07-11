using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BiomeController : MonoBehaviour
{

    public int seed = 1234555;

    List<IBiome> _biomes;

    void Awake()
    {
        _biomes = new List<IBiome>();
        Random.seed = seed;
    }

    // Use this for initialization
    void Start()
    {
        foreach (Component c in transform.GetComponents<Component>())
        {
            if (c is IBiome)
                _biomes.Add((IBiome)c);
        }
    }

    public void GenerateChunk(IChunk chunk, int maxHeight)
    {
        IBiome bg = _biomes[0];

        for (int x = 0; x < chunk.Blocks.GetLength(0); x++)
        {
            for (int y = 0; y < chunk.Blocks.GetLength(1); y++)
            {
                for (int z = 0; z < chunk.Blocks.GetLength(2); z++)
                {
                    if (bg.GetValue(chunk.LocalPositionToWorldPosition(x, y, z), maxHeight) == 1)
                        chunk[x, y, z] = bg.GetBlockFromPosition(chunk.ChunkPosition + new IntVector3(x, y, z), maxHeight);
                    else
                        chunk[x, y, z] = null;
                }
            }
        }

        //Chunk is finished loading
        chunk.IsLoaded = true;
    }

    float BiomeNoise(int x, int z)
    {
        return SimplexNoise.Noise(x * 5, z * 5);

    }

}
