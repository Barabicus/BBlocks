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

    public static IntVector3 operator *(IntVector3 v1, IntVector3 v2)
    {
        return new IntVector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    public static IntVector3 operator *(IntVector3 v1, int v2)
    {
        return new IntVector3(v1.x * v2, v1.y * v2, v1.z * v2);
    }

    public static IntVector3 operator /(IntVector3 v1, IntVector3 v2)
    {
        return new IntVector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
    }

    public static IntVector3 operator /(IntVector3 v1, int i2)
    {
        return new IntVector3(v1.x / i2, v1.y / i2, v1.z / i2);
    }

    public static bool operator ==(IntVector3 i1, IntVector3 i2)
    {
        if (object.ReferenceEquals(i1, null))
        {
            return object.ReferenceEquals(i2, null);
        }

        return i1.Equals(i2);
    }

    public static bool operator !=(IntVector3 i1, IntVector3 i2)
    {
        return !Equals(i1, i2);
    }

    public void Abs()
    {
        x = x < 0 ? x * -1 : x;
        y = y < 0 ? y * -1 : y;
        z = z < 0 ? z * -1 : z;
    }

    public IntVector3 Absolute
    {
        get
        {
            IntVector3 abs = new IntVector3(x, y, z);
            abs.Abs();
            return abs;
        }
    }

    public override string ToString()
    {
        return "(" + x + " : " + y + " : " + z + ")";
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (obj is IntVector3)
        {
            IntVector3 other = (IntVector3)obj;
            return x == other.x && y == other.y && z == other.z;
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            hash = hash * 23 + z.GetHashCode();
            return hash;
        }
    }
    

}
