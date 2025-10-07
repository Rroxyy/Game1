using System.Collections.Generic;
using UnityEngine;


public static class CellConnectionMetrics
{
    public static List<int> LOD0_P2 = new List<int>() { 10,12,17};
    
    public static List<int> LOD0_P3 = new List<int>() { 16,18,23};
    
    
    public static readonly float GapX = HexCellMetrics.outerRadius/2.0f;
    public static readonly float GapZ = GapX / 2 * Mathf.Sqrt(3);

    public static readonly float ConnectionHeight = GapX;
    public static readonly float ConnectionWidth = HexCellMetrics.outerRadius;

    public static readonly List<CellAllDirection> connectionDirections = new List<CellAllDirection>()
    {
        CellAllDirection.Deg0,       //Right
        CellAllDirection.Deg90,      //Down
        CellAllDirection.Deg180,     //Left
        CellAllDirection.Deg270,     //Up
    };

    public static readonly List<Vector3> ConnectionCorners = new List<Vector3>()
    {
        new Vector3(+ConnectionWidth / 2f, 0f, +ConnectionHeight / 2f), // 右上
        new Vector3(+ConnectionWidth / 2f, 0f, -ConnectionHeight / 2f), // 右下
        new Vector3(-ConnectionWidth / 2f, 0f, -ConnectionHeight / 2f), // 左下
        new Vector3(-ConnectionWidth / 2f, 0f, +ConnectionHeight / 2f), // 左上
    };

}