
public struct HexCellCoords
{
    public int x;
    public int z;

    public HexCellCoords(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public override string ToString()
    {
        return $"({x}, {z})";
    }

    #region operator

    public bool Equals(HexCellCoords other)
    {
        return x == other.x && z == other.z;
    }

    public override bool Equals(object obj)
    {
        return obj is HexCellCoords other && Equals(other);
    }


    public static HexCellCoords operator /(HexCellCoords a, (int, int) p)
    {
        return new HexCellCoords(a.x / p.Item1, a.z / p.Item2);
    }

    public static HexCellCoords operator /(HexCellCoords a, int d)
    {
        return new HexCellCoords(a.x / d, a.z / d);
    }

    public static HexCellCoords operator *(HexCellCoords a, int d)
    {
        return new HexCellCoords(a.x * d, a.z * d);
    }


    public static implicit operator (int, int)(HexCellCoords c)
    {
        return (c.x, c.z);
    }

    public static implicit operator HexCellCoords((int, int) t)
    {
        return new HexCellCoords(t.Item1, t.Item2);
    }

    #endregion


    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + x.GetHashCode();
            hash = hash * 31 + z.GetHashCode();
            return hash;
        }
    }
}
