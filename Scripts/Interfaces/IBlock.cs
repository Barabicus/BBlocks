using UnityEngine;
using System.Collections;

public interface IBlock
{
    byte BlockID { get; }
    void ConstructBlock(int x, int y, int z, Chunk chunk);

}
