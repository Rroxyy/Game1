

using UnityEngine;
using UnityEngine.Serialization;

public enum HexCellColorEnum
{
    White,
    Green,
    Blue,
    Yellow,
}

[System.Serializable]
public class HexCellColorItem
{
    public HexCellColorEnum colorEnumType;  // 枚举，不用字符串更安全
    public Color color;  
    
}