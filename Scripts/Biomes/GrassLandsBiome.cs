using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class GrassLandsBiome : MonoBehaviour, IBiome
{

    public AnimationCurve perlinCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    public int octaves = 5;

    public float GetValue(IntVector3 blockPosition, int maxHeight)
    {
        float lands = Mathf.PerlinNoise((blockPosition.x) / (float)maxHeight * octaves, (blockPosition.z) / (float)maxHeight * octaves);
        lands += (maxHeight - blockPosition.y) / (float)maxHeight;
        lands *= perlinCurve.Evaluate((blockPosition.y / (float)maxHeight));
        lands = Mathf.Round(lands);
        lands = Mathf.Clamp(lands, 0f, 1f);
        if (blockPosition.y < 4)
            return 1;
        else
            return lands;
    }

    public Block GetBlockFromPosition(IntVector3 blockPosition, int maxHeight)
    {
        if (blockPosition.y < 2)
            return new StoneBlock();
        else
            if (GetValue(blockPosition + new IntVector3(0, 1, 0), maxHeight) == 0)
                return new GrassBlock();
            else
                return new DirtBlock();
    }

}
