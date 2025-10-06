

using System.Collections.Generic;

public enum TerrainType
{
    Plain,      // 平原
    Mountain,   // 山地
    Hill,       // 丘陵
    Forest,     // 森林
    Desert,     // 沙漠
    Swamp,      // 沼泽
    Water,      // 水域（湖泊/河流/海洋）
    Snow,       // 雪地/冰原
}

public enum LOD_Level
{
    LOD0,
    LOD1,
    LOD2,
}

public enum HexSection
{
    HexCell,
    Connection,
    GapTriangle
}


public abstract class TerrainData
{
    public abstract TerrainType terrainType { get; }

}

