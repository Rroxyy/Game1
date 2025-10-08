using UnityEngine;

public class CellGapTriangle : Cell_Item
{
    public override CellItemType itemType => CellItemType.GapTriangle;

    public HexCell belongToCell { get; private set; }
    public HexCell preDirectionCell { get; private set; }
    public HexCell directionCell { get; private set; }

    private CellBodyDirection _bodyDirection;

    public CellGapTriangle(HexCell _belongToCell, HexCell _preDirectionCell, HexCell _directionCell,
        CellBodyDirection bodyDirection)
    {
        this.belongToCell = _belongToCell;
        this.preDirectionCell = _preDirectionCell;
        this.directionCell = _directionCell;
        this._bodyDirection = bodyDirection;
    }


    public void GetVertices(out Vector3 a, out Vector3 b, out Vector3 c)
    {
        belongToCell.GetVertexByDirection(_bodyDirection, out a);

        var tempDir = CellBodyMetrics.GetPrevioustDirection(_bodyDirection);
        tempDir = CellBodyMetrics.GetInverseDirection(tempDir);
        preDirectionCell.GetVertexByDirection(tempDir, out b);


        tempDir = CellBodyMetrics.GetPrevioustDirection(tempDir);
        tempDir = CellBodyMetrics.GetInverseDirection(tempDir);
        directionCell.GetVertexByDirection(tempDir, out c);
    }

    public override string ToString()
    {
        return "This is a cell gap triangle, belongToCell: " + belongToCell.ToString() +
               " , preDirectionCell: " + preDirectionCell.ToString() +
               " , directionCell: " + directionCell.ToString();
    }
}