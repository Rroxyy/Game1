using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HexCellMapManager : MonoBehaviour
{
    [Header("Base")] 
    [SerializeField] private int heightSize;
    [SerializeField] private int widthSize;

    
    public Dictionary<HexCellCoords, HexCell> hexCellsMap{get; private set;}
    public HexCellQuadtree root;

    [Header("Chunks")]
    public  int chunkSize_Height = 8;
     public  int chunkSize_Width = 8;
    public Dictionary<HexCellCoords,HexCellChunk> chunksMap{get; private set;}


    [SerializeField] private GameObject chunkPrefab;
    
    private HashSet<HexCellChunk> dirtyChunks=new HashSet<HexCellChunk>();

    [Header("Test")] 
    [SerializeField] private bool test = false;


    public static HexCellMapManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
    }


    void Update()
    {
        
    }

    public void UpdateChunks()
    {
        foreach (var chunk in chunksMap.Values)
        {
            chunk.OnUpdate();
        }

        foreach (var chunk in dirtyChunks)
        {
            chunk.RefreshHexCellMesh();
        }
        
        
    }

    private void OnDestroy()
    {
        root = null;
    }

    public void SetDirtyChunk(HexCellChunk chunk)
    {
        dirtyChunks.Add(chunk);
    }

    #region 初始化

     public void GenerateHexMap()
    {
        hexCellsMap = new Dictionary<HexCellCoords, HexCell>();
        chunksMap = new Dictionary<HexCellCoords, HexCellChunk>();

        AABB_Int aabb = new AABB_Int(new Vector3Int(0, 0, 0), new Vector3Int(widthSize - 1, 0, heightSize - 1));
        root = new HexCellQuadtree(aabb, null);
        int x = 0;
        int z = 0;

        
        for ( z = 0; z < heightSize; z++)
        {
            for (x = 0; x < widthSize; x++)
            {
                // base
                float posX = x  * (HexCellMetrics.innerRadius * 2f)+(z & 1)*HexCellMetrics.innerRadius;
                float posZ = z * (HexCellMetrics.outerRadius * 1.5f);

                
                //gap
                posX += HexCellMetrics.GapX*x;
                if (z % 2 != 0)
                {
                    posX+=1.0f*HexCellMetrics.GapX/2f;
                }
                posZ += HexCellMetrics.GapZ*z;

                Vector3 position = new Vector3(posX, 0f, posZ);
                HexCell hexCell = new HexCell(new HexCellCoords(x, z), position,HexCellColorManager.instance.GetColor(HexCellColorEnum.White));
                
                hexCellsMap[new HexCellCoords(x,z)] = hexCell;
                InsertChunkMap(hexCell);
                
                root.Insert(hexCell);
            }
        }

        foreach (var chunk in chunksMap.Values)
        {
            chunk.InitializeChunkMesh();
        }
       
    }

    private void InsertChunkMap(HexCell hexCell)
    {
        var key = hexCell.HexCellCoords / (chunkSize_Width, chunkSize_Height);
        if (!chunksMap.ContainsKey(key))
        {
            var it=Instantiate(chunkPrefab, Vector3.zero,Quaternion.identity,transform);
            var chunk = it.GetComponent<HexCellChunk>();
            chunksMap[key] = chunk;
            Vector3Int min= new Vector3Int(key.x*chunkSize_Width,0,key.z*chunkSize_Height);
            Vector3Int max = new Vector3Int((key.x+1)*chunkSize_Width-1,0,(key.z+1)*chunkSize_Height-1);
            chunk.SetUp(new AABB_Int(min,max));
        }
       
        chunksMap[key].AddHexCell(hexCell);
        hexCell.SetChunk(chunksMap[key]);
    }

    #endregion
   

   

    public HexCellChunk GetChunk(HexCell hexCell)
    {
        return chunksMap[hexCell.HexCellCoords/(chunkSize_Width, chunkSize_Height)];
    }


    #region GetNeighbors

    public void GetCellNeighbors(HexCell center, ref List<HexCell> neighbors, uint radius=2)
    {
        if (radius == 1) return;
        (int cx, int cy, int cz) = HexCellMetrics.OffsetToCube(center.HexCellCoords);

        for (int dx = -(int)radius; dx <= (int)radius; dx++)
        {
            for (int dy = Mathf.Max(-(int)radius, -dx - (int)radius); 
                 dy <= Mathf.Min((int)radius, -dx + (int)radius); dy++)
            {
                int dz = -dx - dy;

                int nx = cx + dx;
                int ny = cy + dy;
                int nz = cz + dz;

                // 转回 offset 坐标
                HexCellCoords neighborCellCoords = HexCellMetrics.CubeToOffset(nx, ny, nz);

                if (hexCellsMap.TryGetValue(neighborCellCoords, out HexCell neighbor))
                {
                    if (neighbor != center) // 排除自己
                        neighbors.Add(neighbor);
                }
            }
        }
    }
    
    //只取周围一格
    public void GetCellNeighbors(HexCell center, ref List<HexCell> neighbors,ref List<HexCellDirection> directions) 
    {
        foreach (var dir in HexCellMetrics.AllDirections)
        {
            var coord=HexCellMetrics.GetHexCellNeighborCoords(center, dir);

            if (hexCellsMap.TryGetValue(coord, out HexCell neighbor))
            {
                neighbors.Add(neighbor);
                directions.Add(dir);
            }
        }
    }

    public HexCell GetCellNeighbors(HexCell center, HexCellDirection direction)
    {
        var coord=HexCellMetrics.GetHexCellNeighborCoords(center, direction);
        if (hexCellsMap.TryGetValue(coord, out HexCell neighbor))
        {
            return neighbor;
        }
        return null;
    }

    #endregion
   
}