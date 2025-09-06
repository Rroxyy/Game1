using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
public class HexCellQuadtree
{
    public AABB_Int aabb_id { get; private set; }
    public AABB aabb_collider { get; private set; }
    private bool aabbCollider_Dirty;


    [SerializeField] private List<HexCellQuadtree> children;
    [SerializeField] private HexCellQuadtree father;
    [SerializeField] private HexCell cell;
    public uint treeHeight { get; private set; }
    public bool isLeaf;

    public HexCellQuadtree(AABB_Int aabbID, HexCellQuadtree _father)
    {
        aabb_id = aabbID;
        aabb_collider = new AABB();
        isLeaf = true;
        treeHeight = 0;
        father = _father;
        cell = null;
        aabbCollider_Dirty = true;
    }

    //插入
    public void Insert(HexCell newCell)
    {
        // aabb_collider.Encapsulate(newCell.GetAABB_Collider());

        if (isLeaf)
        {
            if (cell != null)
            {
                Subdivide();
                Insert(cell);
                cell = null;
                Insert(newCell);
                return;
            }

            cell = newCell;
            cell.SetHexMapQuadtree(this);
            return;
        }

        foreach (var child in children)
        {
            if (child.aabb_id.Contains(new Vector3Int(newCell.HexCellCoords.x, 0, newCell.HexCellCoords.z)))
            {
                child.Insert(newCell);
                break;
            }
        }
    }

    public void GetAllChildren(List<HexCell> list)
    {
        if (isLeaf)
        {
            if (cell != null) list.Add(cell);
            return;
        }

        foreach (var child in children)
        {
            child.GetAllChildren(list);
        }
    }
    

    public HexCell GetMinDistanceHexCellByRay(Ray ray)
    {
        if (!aabb_collider.Contains(ray))
        {
            return null;
        }

        if (isLeaf && cell != null)
        {
            if (cell.Contains(ray))
            {
                return cell;
            }
            else
            {
                return null;
            }
        }
        if (isLeaf && cell == null) return null;

        HexCell ret = null;
        HexCell temp = null;

        foreach (var it in children)
        {
            temp = it.GetMinDistanceHexCellByRay(ray);
            
            
            if (temp != null)
            {
                if (!temp.Contains(ray)) continue;
                if (ret == null)
                {
                    ret = temp;
                    continue;
                }
                else
                {
                    
                    float distance1 = Vector3.Distance(ray.origin, ret.positionWS);
                    float distance2 = Vector3.Distance(ray.origin, temp.positionWS);

                    if (distance1 > distance2)
                    {
                        ret = temp;
                    }
                }
                
            }
        }

        return ret;
    }


    private void Subdivide()
    {
        isLeaf = false;
        children = new List<HexCellQuadtree>();
        Vector3Int center = aabb_id.center;

        // 左下
        children.Add(new HexCellQuadtree(new AABB_Int(
                new Vector3Int(aabb_id.min.x, 0, aabb_id.min.z),
                new Vector3Int(center.x, 0, center.z)),
            this));

        // 右下
        children.Add(new HexCellQuadtree(new AABB_Int(
                new Vector3Int(center.x + 1, 0, aabb_id.min.z),
                new Vector3Int(aabb_id.max.x, 0, center.z)),
            this));

        // 左上
        children.Add(new HexCellQuadtree(new AABB_Int(
                new Vector3Int(aabb_id.min.x, 0, center.z + 1),
                new Vector3Int(center.x, 0, aabb_id.max.z)),
            this));

        // 右上
        children.Add(new HexCellQuadtree(new AABB_Int(
                new Vector3Int(center.x + 1, 0, center.z + 1),
                new Vector3Int(aabb_id.max.x, 0, aabb_id.max.z)),
            this));

        treeHeight = 1;
        father?.UpdateTreeHeight(2);
    }

    private void UpdateTreeHeight(uint _height)
    {
        if (_height > treeHeight)
        {
            treeHeight = _height;
            father?.UpdateTreeHeight(treeHeight + 1);
        }
    }

    public void AABBCollider_Dirty()
    {
        aabbCollider_Dirty = true;
        father?.AABBCollider_Dirty();
    }

    public void UpdateAABB_Collider()
    {
        if (children != null)
        {
            foreach (var it in children)
            {
                it.UpdateAABB_Collider();
            }
        }
        
        
        if (cell != null)
        {
            aabb_collider=cell.GetAABB_Collider();
        }
        else
        {
            aabb_collider.Reset();
        }
        
        if (children != null)
        {
            foreach (var it in children)
            {
                aabb_collider.Encapsulate(it.aabb_collider);
            }
        }

        
        aabbCollider_Dirty = false;
    }

    public void PrintBounds()
    {
        Debug.Log($"Bounds: {aabb_id.min} to {aabb_id.max}");
    }
}