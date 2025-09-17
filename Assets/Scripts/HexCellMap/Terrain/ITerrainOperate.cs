using System.Collections.Generic;
using UnityEngine;

public interface ITerrainOperate
{
    void AddCellMesh(HexCell cell, List<HexCellVertexData> vertexBufferList, List<int> indicesList);
    void AddConnectionMesh(CellConnection connection, List<HexCellVertexData> vertexBufferList, List<int> indicesList,int startIndex);
    void AddGapTriangleMesh(CellGapTriangle gapTriangle, List<HexCellVertexData> vertexBufferList, List<int> indicesList);

    bool ContainsCell(HexCell cell,Ray ray);
    
    bool ContainsConnection(HexCell cell,Ray ray,out CellConnection hitConnection);
    
    bool ContainsGapTriangle(HexCell cell,Ray ray,out CellGapTriangle hitGapTriangle);
}