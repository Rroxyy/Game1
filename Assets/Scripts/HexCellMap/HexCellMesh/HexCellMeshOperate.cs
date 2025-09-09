using System.Collections.Generic;
using UnityEngine;

public struct HexCellVertexData
{
    public Vector3 pos;
    public Vector3 normal;
    public Color32 color;

    public HexCellVertexData(Vector3 pos, Vector3 normal, Color32 color)
    {
        this.pos = pos;
        this.normal = normal;
        this.color = color;
    }
}


public static class HexCellMeshOperate
{
    public static void AddTriangleSubdivide_V2V3(
        Vector3 v1, Vector3 v2, Vector3 v3,
        Color32 color, int subdivisions,
        List<HexCellVertexData> vertexBufferList)
    {
        if (subdivisions <= 1)
        {
            AddTriangle(v1, v2, v3, color, vertexBufferList);
            return;
        }


        Vector3 left=v2;
        Vector3 right;

        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

        for (int i = 0; i < subdivisions; i++)
        {
            float u=(i+1)/(float)subdivisions;
            right = Vector3.Lerp(v2, v3, u);
            AddTriangleWithNormal(v1, left, right, color, normal, vertexBufferList);
            left=right;
        }
    }

    public static void AddTriangleSubdivide_V1Sides(
        Vector3 v1, Vector3 v2, Vector3 v3,
        Color32 color, int subdivisions,
        List<HexCellVertexData> vertexBufferList)
    {
        if (subdivisions <= 1)
        {
            // 不细分，直接绘制一个三角形
            AddTriangle(v1, v2, v3, color, vertexBufferList);
            return;
        }

        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

        Vector3 startLeft =v1;
        Vector3 startRight=v1;
        Vector3 endLeft   ;
        Vector3 endRight  ;
        // 构造小三角形
        for (int i = 0; i < subdivisions; i++)
        {
            float next=(i+1)/(float)subdivisions;
            // 这一层的起点和下一层的起点
            endLeft = Vector3.Lerp(v1, v2, next);
            endRight = Vector3.Lerp(v1, v3, next);

            if (i == 0)
            {
                AddTriangleWithNormal(v1,endLeft,endRight,color, normal,vertexBufferList);
            }
            else
            {
                AddQuadWithNormal(startRight, startLeft, endLeft, endRight, color,normal, vertexBufferList);
            }
            startLeft=endLeft;
            startRight=endRight;
            
        }
    }


    public static void AddTriangleSubdivide_AllEdges(
        Vector3 v1, Vector3 v2, Vector3 v3,
        Color32 color, 
        int subdivisionsV2V3,
        int subdivisionsV1Sides,
        List<HexCellVertexData> vertexBufferList)
    {
        if (subdivisionsV1Sides <= 1 && subdivisionsV2V3 <= 1)
        {
            AddTriangle(v1, v2, v3, color, vertexBufferList);
            return;
        }
        
        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;
        float next;
        Vector3 startLeft  =v1;
        Vector3 startRight =v1;
        Vector3 endLeft    ;
        Vector3 endRight   ;
        Vector3 i1;
        Vector3 i2;
        Vector3 j1;
        Vector3 j2;
        // 构造小三角形
        for (int i =0; i < subdivisionsV1Sides; i++)
        {
           
            next = (i+1) /  (float)subdivisionsV1Sides;
            
            // 这一层的起点和下一层的起点
            
            endLeft    = Vector3.Lerp(v1, v2, next);
            endRight   = Vector3.Lerp(v1, v3, next);

            if (i == 0)
            {
                AddTriangleSubdivide_V2V3(v1, endLeft, endRight, color, subdivisionsV2V3, vertexBufferList);
            }
            else
            {
                i1 = startLeft;
                j1 = endLeft;

                for (int j = 0; j < subdivisionsV2V3; j++)
                {
                    next = (j+1) / (float)subdivisionsV2V3;
                
                    i2=Vector3.Lerp(startLeft, startRight, next);
                    j2=Vector3.Lerp(endLeft, endRight, next);
                
                
                    AddQuadWithNormal(i2,i1,j1,j2, color,normal, vertexBufferList);
                
                    i1=i2;
                    j1=j2;

                }
            }
            startLeft=endLeft;
            startRight=endRight;

            
        }
    }

    public static void AddQuadWithNormal(
        Vector3 a2, Vector3 a1, Vector3 b2, Vector3 b1, 
        Color32 c,
        Vector3 normal,
        List<HexCellVertexData> vertexBufferList)
    {
        AddTriangleWithNormal(a1, b2, b1, c, normal,vertexBufferList);
        AddTriangleWithNormal(b1, a2, a1, c,normal, vertexBufferList);
    }
    
    public static void AddQuadWithNormal(
        Vector3 a2, Vector3 a1, Vector3 b2, Vector3 b1, 
        Color32 c1, Color32 c2,
        Vector3 normal,
        List<HexCellVertexData> vertexBufferList)
    {
        AddTriangleWithNormal(a1, b2, b1, c1, c2, c2, normal,vertexBufferList);
        AddTriangleWithNormal(b1, a2, a1, c2, c1, c1,normal, vertexBufferList);
    }

    public static void AddQuad(Vector3 a2, Vector3 a1, Vector3 b2, Vector3 b1, Color32 c1, Color32 c2,
        List<HexCellVertexData> vertexBufferList)
    {
        AddTriangle(a1, b2, b1, c1, c2, c2, vertexBufferList);
        AddTriangle(b1, a2, a1, c2, c1, c1, vertexBufferList);
    }



    #region Add Triangle

    public static void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color32 color,
        List<HexCellVertexData> vertexBufferList) => AddTriangle(v1, v2, v3, color, color, color, vertexBufferList);

    public static void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color32 c1, Color32 c2, Color32 c3,
        List<HexCellVertexData> vertexBufferList)
    {
        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;
        AddSingleVertex(v1, normal, c1, vertexBufferList);
        AddSingleVertex(v2, normal, c2, vertexBufferList);
        AddSingleVertex(v3, normal, c3, vertexBufferList);
    }


    private static void AddTriangleWithNormal(
        Vector3 v1, Vector3 v2, Vector3 v3,
        Color32 color, Vector3 normal,
        List<HexCellVertexData> vertexBufferList)
    {
        AddSingleVertex(v1, normal, color, vertexBufferList);
        AddSingleVertex(v2, normal, color, vertexBufferList);
        AddSingleVertex(v3, normal, color, vertexBufferList);
    }
    
    private static void AddTriangleWithNormal(
        Vector3 v1, Vector3 v2, Vector3 v3,
        Color32 color1, Color32 color2,Color32 color3,
        Vector3 normal,
        List<HexCellVertexData> vertexBufferList)
    {
        AddSingleVertex(v1, normal, color1, vertexBufferList);
        AddSingleVertex(v2, normal, color2, vertexBufferList);
        AddSingleVertex(v3, normal, color3, vertexBufferList);
    }
    
    #endregion
    
    public static void AddSingleVertex(Vector3 pos, Vector3 normal, Color32 color,
        List<HexCellVertexData> vertexBufferList)
    {
        vertexBufferList.Add(new HexCellVertexData(pos, normal, color));
    }
}