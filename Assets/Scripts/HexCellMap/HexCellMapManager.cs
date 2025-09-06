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

    
    public Dictionary<(int, int), HexCell> hexCellsMap{get; private set;}
    public HexCellQuadtree root;

    [Header("Chunks")]
    public  int chunkCellHeight = 8;
    public  int chunkCellWidth = 8;
    public Dictionary<(int,int),HexCellChunk> chunksMap{get; private set;}


    [SerializeField] private GameObject chunkPrefab;

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
        if (test)
        {
            
        }
    }

    private void OnDestroy()
    {
        root = null;
    }


    public void GenerateHexMap()
    {
        hexCellsMap = new Dictionary<(int, int), HexCell>();
        chunksMap = new Dictionary<(int, int), HexCellChunk>();

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
                
                hexCellsMap[(x, z)] = hexCell;
                InsertChunkMap(hexCell);
                
                root.Insert(hexCell);
            }
        }
    }

    private void InsertChunkMap(HexCell hexCell)
    {
        var key=(hexCell.HexCellCoords.x/chunkCellWidth, hexCell.HexCellCoords.z/chunkCellHeight);
        if (chunksMap.ContainsKey(key))
        {
            chunksMap[key].AddHexCell(hexCell);
        }
        else
        {
            var it=Instantiate(chunkPrefab, Vector3.zero,Quaternion.identity,transform);
            var chunk = it.GetComponent<HexCellChunk>();
            chunksMap[key] = chunk;
            chunk.AddHexCell(hexCell);
        }
    }

   

    public HexCellChunk GetChunk(HexCell hexCell)
    {
        (int, int) pos = hexCell.HexCellCoords;
        
        return chunksMap[(pos.Item1/chunkCellHeight,pos.Item2/chunkCellWidth)];
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

                if (hexCellsMap.TryGetValue((neighborCellCoords.x, neighborCellCoords.z), out HexCell neighbor))
                {
                    if (neighbor != center) // 排除自己
                        neighbors.Add(neighbor);
                }
            }
        }
    }
    
    //支取周围一格
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