using UnityEngine;
using System.Collections;

public class DebugBiome : MonoBehaviour, IBiome
{

    public AnimationCurve perlinCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    public int octaves = 5;

    public float GetValue(IntVector3 blockPosition, int maxHeight)
    {
        if (blockPosition.y < 100)
            return 1;
        else
            return 0;
    }

    public Block GetBlockFromPosition(IntVector3 blockPosition, int maxHeight)
    {
        return new StoneBlock();
    }
}
