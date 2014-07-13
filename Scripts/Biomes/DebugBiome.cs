using UnityEngine;
using System.Collections;

public class DebugBiome : MonoBehaviour, IBiome
{

    public AnimationCurve perlinCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    public int octaves = 5;

    public float GetValue(IntVector3 blockPosition, int maxHeight)
    {
        return 1;
    }

    public Block GetBlockFromPosition(IntVector3 blockPosition, int maxHeight)
    {
        return new StoneBlock();
    }
}
