using UnityEngine;
using System.Collections;

public interface IWorldAnchor
{
    IntVector3 AnchorPosition { get; }
    IntVector3 AnchorBounds { get; }

}
