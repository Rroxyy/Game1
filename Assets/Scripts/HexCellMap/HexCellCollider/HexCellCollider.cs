using UnityEngine;

public class HexCellCollider
{
    public AABB aabb { get;private set; }
    private bool dirty=true;
    public HexCell cell{ get; private set; }
    private HexCellQuadtree quadtree;

    public HexCellCollider(HexCell _cell)
    {
        cell = _cell;
        aabb = new AABB();
        dirty = true;
    }
    
    public void SetQuadTree(HexCellQuadtree _tree)=> quadtree = _tree;

    public void SetDirty(bool setNeighborDirty=true)
    {
        dirty = true;
        quadtree.AABBCollider_Dirty();
        if (!setNeighborDirty) return;
        foreach (var dir in CellBodyMetrics.HalfDirections)
        {
            var neighbor = HexCellMapManager.instance.GetCellNeighbors(cell, dir);
            neighbor?.hexCellCollider.SetDirty(false);
        }
    }

    public AABB GetAABB()
    {
        RefreshAABB();
        return aabb;
    }
    public void RefreshAABB()
    {
        if (!dirty) return;
        aabb.Reset();
        
        aabb.Encapsulate(CellBodyMetrics.GetAABB(cell.positionWS));

        foreach (var dir in CellBodyMetrics.AllDirections)
        {
            var neighbor = HexCellMapManager.instance.GetCellNeighbors(cell, dir);
            if (neighbor == null) continue;
            neighbor.GetVertexByDirection(CellBodyMetrics.GetInverseDirection(dir),out var p1, out var p2);
            cell.GetVertexByDirection(dir, out var a1, out var a2);
            
            aabb.Encapsulate((p2+a1)/2.0f);
            aabb.Encapsulate((p1+a2)/2.0f);
        }

        dirty = false;

    }
    
   


}