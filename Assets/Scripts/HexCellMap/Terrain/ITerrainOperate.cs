
    using System.Collections.Generic;
    using UnityEngine;

    public interface ITerrainOperate
    {
        void AddCellMesh(HexCell cell, List<HexCellVertexData> vertexBufferList);
        void AddConnectionMesh(HexCell cell, List<HexCellVertexData> vertexBufferList);
        void AddGapTriangleMesh(HexCell cell, List<HexCellVertexData> vertexBufferList);
    }
