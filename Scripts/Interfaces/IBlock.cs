using UnityEngine;
using System.Collections;

public interface IBlock
{
    byte BlockID { get; }
    void ConstructBlock(int x, int y, int z, Chunk chunk);

    bool IsOpaque { get; }

    void BlockDestroyed(Chunk chunk);

    void BlockPlaced(Chunk chunk);

}
