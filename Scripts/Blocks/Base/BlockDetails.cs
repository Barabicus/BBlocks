using UnityEngine;
using System.Collections;

public sealed class BlockDetails
{

    public const byte StoneBlock = 0;
    public const byte GrassBlock = 1;
    public const byte DirtBlock = 2;
    public const byte UnbreakableBlock = 3;
    public const byte GrassBlockSlanted = 4;

    public const float tUnit = 0.0625f;
    public static Vector2 nullUV = Vector2.zero;
    public static Vector2 stoneUV = new Vector2(1, 15);
    public static Vector2 grassUV = new Vector2(1, 6);
    public static Vector2 grassSideUV = new Vector2(3, 15);
    public static Vector2 dirtUV = new Vector2(2, 15);
    public static Vector2 unbreakableUV = new Vector2(5, 13);

}
