using System.Collections.Generic;
using UnityEngine;

public class LOD0_PlainTerrain : ITerrainOperate
{
    public void AddCellMesh(HexCell cell, List<HexCellVertexData> vertexBufferList, List<int> indicesList)
    {
        Vector3 positionWS = cell.positionWS;
        for (int i = 0; i < 6; i++)
        {
            HexCellMeshOperate.AddTriangleSubdivide_AllEdges(
                positionWS,
                cell.GetVertexByIndex(i),
                cell.GetVertexByIndex(i + 1),
                cell.cellColor, cell.cellColor, cell.cellColor,
                4, 2,
                vertexBufferList,
                indicesList
            );
        }
    }

    public void AddConnectionMesh(CellConnection connection, List<HexCellVertexData> vertexBufferList,
        List<int> indicesList)
    {
        var cell = connection.BelongsToHexCell;
        connection.GetVertices(out var a2, out var a1, out var b2, out var b1);
        Vector3 normal = Vector3.Cross((b2 - a1), (b1 - a1)).normalized;

        HexCellMeshOperate.AddQuadSubdivide_AllEdges(a2, a1, b2, b1,
            cell.cellColor, cell.cellColor, connection.OtherHexCell.cellColor, connection.OtherHexCell.cellColor,
            4, 2,
            normal,
            vertexBufferList, indicesList
        );
    }

    public void AddGapTriangleMesh(CellGapTriangle gapTriangle, List<HexCellVertexData> vertexBufferList,
        List<int> indicesList)
    {
        var cell = gapTriangle.belongToCell;
        gapTriangle.GetVertices(out var a, out var b, out var c);
        HexCellMeshOperate.AddTriangleSubdivide_AllEdges(
            a, b, c,
            cell.cellColor, gapTriangle.preDirectionCell.cellColor, gapTriangle.directionCell.cellColor,
            2, 2,
            vertexBufferList,
            indicesList
        );
    }

    public bool ContainsCell(HexCell cell, Ray ray)
    {
        Plane plane = new Plane(Vector3.up, cell.positionWS);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return ContainsCell(cell, hitPoint);
        }


        return false;
    }

    private bool ContainsCell(HexCell cell, Vector3 point)
    {
        Vector3 normal = Vector3.up;

        bool? inside = null;

        for (int i = 0; i < 6; i++)
        {
            Vector3 a = cell.GetVertexByIndex(i);
            Vector3 b = cell.GetVertexByIndex(i + 1);
            ;

            Vector3 ab = b - a;
            Vector3 ap = point - a;

            Vector3 cross = Vector3.Cross(ab, ap);

            float dot = Vector3.Dot(cross, normal);

            if (i == 0)
            {
                inside = dot >= 0;
            }
            else
            {
                if ((dot >= 0) != inside.Value)
                    return false;
            }
        }

        return true;
    }

    public bool ContainsConnection(HexCell cell, Ray ray, out CellConnection hitConnection)
    {
        foreach (var dir in HexCellMetrics.AllDirections)
        {
            var connection = cell.GetConnectionByDirection(dir);
            if (connection != null)
            {
                connection.GetVertices(out var a2, out var a1, out var b2, out var b1);
                Vector3 normal = Vector3.Cross((b2 - a1), (b1 - a1)).normalized;
                Plane plane = new Plane(normal, a1);

                if (plane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);

                    if (Triangle.PointInTriangle(hitPoint, a1, b2, b1) ||
                        Triangle.PointInTriangle(hitPoint, b1, a2, a1))
                    {
                        hitConnection = connection;
                        return true;
                    }
                }
            }
        }

        hitConnection = null;
        return false;
    }

    public bool ContainsGapTriangle(HexCell cell, Ray ray, out CellGapTriangle hitGapTriangle)
    {
        foreach (var dir in HexCellMetrics.AllDirections)
        {
            var gapTriangle = cell.GetCellGapTriangleByDirection(dir);
            if (gapTriangle != null)
            {
                gapTriangle.GetVertices(out var a, out var b, out var c);

                var normal = Vector3.Cross((b - a), (c - a)).normalized;
                Plane plane = new Plane(normal, a);
                if (plane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);

                    if (Triangle.PointInTriangle(hitPoint, a, b, c))
                    {
                        hitGapTriangle = gapTriangle;
                        return true;
                    }
                }
            }
        }

        hitGapTriangle = null;
        return false;
    }
}