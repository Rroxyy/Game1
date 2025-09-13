using UnityEngine;

public class HexCellCollider
{
    public AABB aabb { get;private set; }

    private HexCell cell;
    
    private bool dirty=true;

    public HexCellCollider(HexCell _cell)
    {
        cell = _cell;
        aabb = new AABB();
        dirty = true;
    }

    public void SetDirty(bool setNeighborDirty=true)
    {
        dirty = true;
        if (!setNeighborDirty) return;
        foreach (var dir in HexCellMetrics.HalfDirections)
        {
            var neighbor = HexCellMapManager.instance.GetCellNeighbors(cell, dir);
            neighbor?.hexCellCollider.SetDirty(false);
        }
    }
    public void RefreshAABB()
    {
        if (!dirty) return;
        aabb.Reset();
        
        aabb.Encapsulate(HexCellMetrics.GetAABB(cell.positionWS));

        foreach (var dir in HexCellMetrics.AllDirections)
        {
            var neighbor = HexCellMapManager.instance.GetCellNeighbors(cell, dir);
            if (neighbor == null) continue;
            neighbor.GetVertexByDirection(HexCellMetrics.GetInverseDirection(dir),out var p1, out var p2);
            cell.GetVertexByDirection(dir, out var a1, out var a2);
            
            aabb.Encapsulate((p2+a1)/2.0f);
            aabb.Encapsulate((p1+a2)/2.0f);
        }

        dirty = false;

    }

}