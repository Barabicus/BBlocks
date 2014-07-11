using UnityEngine;
using System.Collections;

public interface IBiome
{

    Block GetBlockFromPosition(IntVector3 blockPosition, int maxHeight);

    float GetValue(IntVector3 blockPosition, int maxHeight);
}
