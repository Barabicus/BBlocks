using UnityEngine;
using System.Collections;

/// <summary>
/// Simple struct for storing 3 point ints.
/// </summary>
public struct IntVector3
{

    public int x, y, z;

    /// <summary>
    /// Takes in a Vector3 and converts it into a IntVector3
    /// </summary>
    /// <param name="vector"></param>
    public IntVector3(Vector3 vector)
    {
        x = Mathf.RoundToInt(vector.x);
        y = Mathf.RoundToInt(vector.y);
        z = Mathf.RoundToInt(vector.z);
    }

    public IntVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public IntVector3(float x, float y, float z)
    {
        this.x = Mathf.RoundToInt(x);
        this.y = Mathf.RoundToInt(y);
        this.z = Mathf.RoundToInt(z);
    }

    /// <summary>
    /// Returns this vector back as a Vector3
    /// </summary>
    /// <returns></returns>
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public static IntVector3 operator -(IntVector3 v1, IntVector3 v2)
    {
        return new IntVector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }

    public static IntVector3 operator +(IntVector3 v1, IntVector3 v2)
    {
        return new IntVector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public static IntVector3 operator /(IntVector3 v1, IntVector3 v2)
    {
        return new IntVector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
    }

    public static IntVector3 operator /(IntVector3 v1, int i2)
    {
        return new IntVector3(v1.x / i2, v1.y / i2, v1.z / i2);
    }

    public override string ToString()
    {
        return "(" + x + " : " + y + " : " + z + ")";
    }

}
