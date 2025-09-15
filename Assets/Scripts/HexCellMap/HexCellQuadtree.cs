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
    // [SerializeField] private HexCell cell;
    [SerializeField] private HexCellCollider cellCollider;
    public uint treeHeight { get; private set; }
    public bool isLeaf;

    public HexCellQuadtree(AABB_Int aabbID, HexCellQuadtree _father)
    {
        aabb_id = aabbID;
        aabb_collider = new AABB();
        isLeaf = true;
        treeHeight = 0;
        father = _father;
        // cell = null;
        cellCollider = null;
        aabbCollider_Dirty = true;
    }

    //插入
    public void Insert(HexCellCollider newCollider)
    {
        // aabb_collider.Encapsulate(newCell.GetAABB_Collider());

        if (isLeaf)
        {
            if (cellCollider != null)
            {
                Subdivide();
                Insert(cellCollider);
                
                cellCollider = null;
                Insert(newCollider);
                return;
            }

            cellCollider = newCollider;
            cellCollider.SetQuadTree(this);
            return;
        }

        foreach (var child in children)
        {
            var newCell = newCollider.cell;
            if (child.aabb_id.Contains(new Vector3Int(newCell.hexCellCoords.x, 0, newCell.hexCellCoords.z)))
            {
                child.Insert(newCollider);
                break;
            }
        }
    }

   


    #region Collider AABB methods

    public AABB GetCombinedCollider(AABB_Int queryAABB)
    {
        if (aabbCollider_Dirty)
        {
            UpdateAABB_Collider();
        }
        
        // 当前节点的 aabb_id 与 queryAABB 完全不重合
        if (!aabb_id.Overlaps(queryAABB))
        {
            return null; 
        }

        // 如果是叶子节点，直接返回当前节点的 aabb_collider
        if (isLeaf)
        {
            return cellCollider == null ? null : new AABB(cellCollider.GetAABB());
        }

        // 如果是非叶子节点，递归子节点
        AABB combined = null;
        bool hasValid = false;

        foreach (var child in children)
        {
            var childAABB = child.GetCombinedCollider(queryAABB);
            if (childAABB != null) // 有有效 collider
            {
                if (!hasValid)
                {
                    combined = childAABB;
                    hasValid = true;
                }
                else
                {
                    combined.Encapsulate(childAABB);
                }
            }
        }

        return hasValid ? combined : null;
    }
    public HexCell GetMinDistanceHexCellByRay(Ray ray)
    {
        if (aabbCollider_Dirty)
        {
            UpdateAABB_Collider();
        }
        
        if (!aabb_collider.Contains(ray))
        {
            return null;
        }

        if (isLeaf && cellCollider != null)
        {
            return cellCollider.cell;
        }
        if (isLeaf && cellCollider == null) return null;

        HexCell ret = null;
        HexCell temp = null;

        foreach (var it in children)
        {
            temp = it.GetMinDistanceHexCellByRay(ray);
            
            
            if (temp != null)
            {
                if (ret == null)
                {
                    ret = temp;
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

    
    public void AABBCollider_Dirty()
    {
        if (aabbCollider_Dirty) return;
        aabbCollider_Dirty = true;
        father?.AABBCollider_Dirty();
    }

    public void UpdateAABB_Collider()
    {
        if (!aabbCollider_Dirty) return;
        
        if (children != null)
        {
            foreach (var it in children)
            {
                it.UpdateAABB_Collider();
            }
        }
        
        
        if (cellCollider != null)
        {
            aabb_collider .Reset(cellCollider.GetAABB()); 
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
    
    #endregion
    


   

    



   

    #region Basically not used(initialize function)
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

    #endregion
   
}