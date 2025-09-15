using System.Collections.Generic;
using UnityEngine;

public class CellConnection: Cell_Item
{
    public override CellItemType itemType => CellItemType.Connection;

    public HexCell BelongsToHexCell{get;private set;}
    public HexCell OtherHexCell{get;private set;}
    public HexCellDirection direction { get;private set; }

    public CellConnection(HexCell _BelongsToHexCell, HexCell _OtherHexCell,HexCellDirection _dir)
    {
        BelongsToHexCell = _BelongsToHexCell;
        OtherHexCell = _OtherHexCell;
        direction = _dir;
    }

    public void GetVertices(out Vector3 a2,out Vector3 a1,  out Vector3 b2, out Vector3 b1)
    {
        BelongsToHexCell.GetVertexByDirection(direction, out  a1, out a2);
        OtherHexCell.GetVertexByDirection(HexCellMetrics.GetInverseDirection(direction), out  b1, out b2);
       
    }

    public override string ToString()
    {
        return "Cell Connection, BelongsToHexCell: "+BelongsToHexCell.ToString()+" , "+"OtherHexCell: "+OtherHexCell.ToString();
    }

}