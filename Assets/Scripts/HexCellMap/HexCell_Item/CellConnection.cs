using System.Collections.Generic;
using UnityEngine;

public class CellConnection
{
    public HexCell BelongsToHexCell{get;private set;}
    public HexCell OtherHexCell{get;private set;}
    public HexCellDirection direction { get;private set; }

    public CellConnection(HexCell cell1, HexCell cell2,HexCellDirection _dir)
    {
        BelongsToHexCell = cell1;
        OtherHexCell = cell2;
        direction = _dir;
    }

    public void GetVertices(out Vector3 a2,out Vector3 a1,  out Vector3 b2, out Vector3 b1)
    {
        BelongsToHexCell.GetVertexByDirection(direction, out  a1, out a2);
        OtherHexCell.GetVertexByDirection(HexCellMetrics.GetInverseDirection(direction), out  b1, out b2);
       
    }
}