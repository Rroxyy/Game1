using System.Collections.Generic;
using UnityEngine;

public interface ITerrainOperate
{
    void AddCellMesh(HexCell cell, List<HexCellVertexData> vertexBufferList, List<int> indicesList);
    void AddConnectionMesh(HexCell cell, List<HexCellVertexData> vertexBufferList, List<int> indicesList);
    void AddGapTriangleMesh(HexCell cell, List<HexCellVertexData> vertexBufferList, List<int> indicesList);

    bool Contains(HexCell cell,Ray ray);
    bool Contains(HexCell cell,Vector3 point);
}