using UnityEngine;

public class CellGapTriangle : Cell_Item
{
    public override CellItemType itemType => CellItemType.GapTriangle;

    public HexCell belongToCell { get; private set; }
    public HexCell preDirectionCell { get; private set; }
    public HexCell directionCell { get; private set; }

    private HexCellDirection direction;

    public CellGapTriangle(HexCell _belongToCell, HexCell _preDirectionCell, HexCell _directionCell,
        HexCellDirection _direction)
    {
        this.belongToCell = _belongToCell;
        this.preDirectionCell = _preDirectionCell;
        this.directionCell = _directionCell;
        this.direction = _direction;
    }


    public void GetVertices(out Vector3 a, out Vector3 b, out Vector3 c)
    {
        belongToCell.GetVertexByDirection(direction, out a);

        var tempDir = HexCellMetrics.GetPrevioustDirection(direction);
        tempDir = HexCellMetrics.GetInverseDirection(tempDir);
        preDirectionCell.GetVertexByDirection(tempDir, out b);


        tempDir = HexCellMetrics.GetPrevioustDirection(tempDir);
        tempDir = HexCellMetrics.GetInverseDirection(tempDir);
        directionCell.GetVertexByDirection(tempDir, out c);
    }

    public override string ToString()
    {
        return "This is a cell gap triangle, belongToCell: " + belongToCell.ToString() +
               " , preDirectionCell: " + preDirectionCell.ToString() +
               " , directionCell: " + directionCell.ToString();
    }
}