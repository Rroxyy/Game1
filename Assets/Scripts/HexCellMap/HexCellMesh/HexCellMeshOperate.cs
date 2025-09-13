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
    
    
    
    #region AddTriangleSubdivide

   public static void AddTriangleSubdivide_V2V3(
        Vector3 v1, Vector3 v2, Vector3 v3,
        Color32 c1, Color32 c2, Color32 c3, 
        int subdivisions,
        List<HexCellVertexData> vertexBufferList,
        List<int> indicesList
        )
    {
        if (subdivisions <= 1)
        {
            AddTriangle(v1, v2, v3, c1, vertexBufferList);
            for (int i = 0; i < 3; i++)
            {
                indicesList.Add(indicesList.Count);
            }
            return;
        }


        Vector3 left=v2;
        Color32 leftColor = c2;
        Vector3 right;
        Color32 rightColor;

        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

        for (int i = 0; i < subdivisions; i++)
        {
            float u=(i+1)/(float)subdivisions;
            rightColor=Color32.Lerp(c2,c3,u);
            right = Vector3.Lerp(v2, v3, u);
            AddTriangleWithNormal(v1, left, right, c1,leftColor,rightColor, normal, vertexBufferList);
            for (int j = 0; j < 3; j++)
            {
                indicesList.Add(indicesList.Count);
            }
            
            left=right;
            leftColor = rightColor;
        }
    }

    public static void AddTriangleSubdivide_V1Sides(
        Vector3 v1, Vector3 v2, Vector3 v3,
        Color32 c1, Color32 c2, Color32 c3, 
        int subdivisions,
        List<HexCellVertexData> vertexBufferList,
        List<int> indicesList)
    {
        if (subdivisions <= 1)
        {
            // 不细分，直接绘制一个三角形
            AddTriangle(v1, v2, v3, c1, vertexBufferList);
            for (int j = 0; j < 3; j++)
            {
                indicesList.Add(indicesList.Count);
            }
            return;
        }

        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

        Vector3 startLeft =v1;
        Vector3 startRight=v1;
        Vector3 endLeft   ;
        Vector3 endRight  ;
        
        Color32 startColorLeft = c1;
        Color32 startColorRight = c1;
        Color32 endColorLeft  ;
        Color32 endColorRight ;
        
        for (int i = 0; i < subdivisions; i++)
        {
            float next=(i+1)/(float)subdivisions;
            endLeft = Vector3.Lerp(v1, v2, next);
            endRight = Vector3.Lerp(v1, v3, next);

            endColorLeft = Color32.Lerp(c1, c2, next);
            endColorRight = Color32.Lerp(c1, c3, next);

            if (i == 0)
            {
                AddTriangleWithNormal(v1,endLeft,endRight,c1,endColorLeft,endColorRight, normal,vertexBufferList);
                for (int j = 0; j < 3; j++)
                {
                    indicesList.Add(indicesList.Count);
                }
            }
            else
            {
                AddQuadWithNormal(startRight, startLeft, endLeft, endRight,startColorRight,startColorLeft,endColorLeft,endColorRight,normal, vertexBufferList);
                
                for (int j = 0; j < 6; j++)
                {
                    indicesList.Add(indicesList.Count);
                }
            }
            startLeft=endLeft;
            startRight=endRight;

            startLeft=endLeft;
            startRight = endRight;

        }
    }


    public static void AddTriangleSubdivide_AllEdges(
        Vector3 v1, Vector3 v2, Vector3 v3,
        Color32 c1, Color32 c2, Color32 c3, 
        int subdivisionsV2V3,
        int subdivisionsV1Sides,
        List<HexCellVertexData> vertexBufferList,
        List<int> indicesList)
    {
        if (subdivisionsV1Sides <= 1 && subdivisionsV2V3 <= 1)
        {
            AddTriangle(v1, v2, v3, c1,c2,c3, vertexBufferList);
            for (int j = 0; j < 3; j++)
            {
                indicesList.Add(indicesList.Count);
            }
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
        
        Color32 startColorLeft = c1;
        Color32 startColorRight = c1;
        Color32 endColorLeft  ;
        Color32 endColorRight ;
        
        Color32 midStartLeft;
        Color32 midStartRight;
        Color32 midEndLeft;
        Color32 midEndRight;
        // 构造小三角形
        for (int i =0; i < subdivisionsV1Sides; i++)
        {
           
            next = (i+1) /  (float)subdivisionsV1Sides;
            
            
            endLeft    = Vector3.Lerp(v1, v2, next);
            endRight   = Vector3.Lerp(v1, v3, next);
            
            endColorLeft = Color.Lerp(c1, c2, next);
            endColorRight = Color.Lerp(c1, c3, next);

            if (i == 0)
            {
                AddTriangleSubdivide_V2V3(v1, endLeft, endRight, c1,endColorLeft,endColorRight, subdivisionsV2V3, vertexBufferList,indicesList);
            }
            else
            {
                i1 = startLeft;
                j1 = endLeft;

                midStartLeft = startColorLeft;
                midEndLeft = endColorLeft;

                for (int j = 0; j < subdivisionsV2V3; j++)
                {
                    next = (j+1) / (float)subdivisionsV2V3;
                
                    i2=Vector3.Lerp(startLeft, startRight, next);
                    j2=Vector3.Lerp(endLeft, endRight, next);
                    
                    midStartRight=Color.Lerp(startColorLeft, startColorRight, next);
                    midEndRight=Color.Lerp(endColorLeft, endColorRight, next);
                
                
                    AddQuadWithNormal(i2,i1,j1,j2, midStartRight,midStartLeft,midEndLeft,midEndRight,normal, vertexBufferList);
                    
                    
                    for (int k = 0; k < 6; k++)
                    {
                        indicesList.Add(indicesList.Count);
                    }
                    i1=i2;
                    j1=j2;

                    midStartLeft = midStartRight;
                    midEndLeft = midEndRight;

                }
            }
            startLeft=endLeft;
            startRight=endRight;

            startColorLeft = endColorLeft;
            startColorRight = endColorRight;
        }
    }

    #endregion


    #region AddQuadSubdivide

    public static void AddQuadSubdivide_AllEdges(Vector3 a2, Vector3 a1, Vector3 b2, Vector3 b1,
        Color32 c1, Color32 c2, Color32 c3, Color32 c4,
        int subdivisionsA1A2,
        int subdivisionsA1B1,
        Vector3 normal, 
        List<HexCellVertexData> vertexBufferList,
        List<int> indicesList)
    {
        if (subdivisionsA1A2 <= 1 && subdivisionsA1B1 <= 1)
        {
            AddQuadWithNormal(a2, a1, b2, b1,c1,c2,c3,c4,normal,vertexBufferList);
            for (int i = 0; i < 6; i++)
            {
                indicesList.Add(indicesList.Count);
            }
        }

        Vector3 startLeft = a1;
        Vector3 startRight = a2;
        Vector3 endLeft;
        Vector3 endRight;
        
        Color32 startColorLeft = c2;
        Color32 startColorRight = c1;
        Color32 endColorLeft ;
        Color32 endColorRight;
        
        Vector3 midStartLeft;
        Vector3 midStartRight;
        Vector3 midEndLeft;
        Vector3 midEndRight;

        Color32 midColorStartLeft;
        Color32 midColorStartRight;
        Color32 midColorEndLeft;
        Color32 midColorEndRight;

        for (int i = 0; i < subdivisionsA1B1; i++)
        {
            float ne = (i + 1)/ (float)subdivisionsA1B1;
            endLeft=Vector3.Lerp(a1,b2, ne);
            endRight = Vector3.Lerp(a2, b1, ne);
            
            endColorLeft=Color.Lerp(c2, c3, ne);
            endColorRight=Color.Lerp(c1, c4, ne);


            midStartLeft = startLeft;
            midEndLeft=endLeft;

            midColorStartLeft = startColorLeft;
            midColorEndLeft=endColorLeft;

            for (int j = 0; j < subdivisionsA1A2; j++)
            {
                ne = (j + 1)/ (float)subdivisionsA1A2;
                
                midStartRight=Vector3.Lerp(startLeft, startRight, ne);
                midEndRight=Vector3.Lerp(endLeft, endRight, ne);
                
                midColorStartRight=Color.Lerp(startColorLeft,startColorRight, ne);
                midColorEndRight=Color.Lerp(endColorLeft, endColorRight, ne);
                
                AddQuadWithNormal(midStartRight,midStartLeft,midEndLeft,midEndRight,
                    midColorStartRight,midColorStartLeft,midColorEndLeft,midColorEndRight,
                    normal,vertexBufferList
                    );
                
                for(int k=0;k<6;k++)indicesList.Add(indicesList.Count);
                
                midStartLeft=midStartRight;
                midEndLeft=midEndRight;
                
                midColorStartLeft=midColorStartRight;
                midColorEndLeft=midColorEndRight;
            }
            


            startLeft = endLeft;
            startRight = endRight;
            
            startColorLeft=endColorLeft;
            startColorRight=endColorRight;
        }
        
    }

    #endregion

    #region Add Hex Cell

    public static void AddSingleHexCell(HexCell cell, List<HexCellVertexData> vertexBufferList,List<int>indicesList)
    {
        var positionWS = cell.positionWS;
        
    }

    #endregion
    

    #region Add Quad

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
    
    public static void AddQuadWithNormal(
        Vector3 a2, Vector3 a1, Vector3 b2, Vector3 b1, 
        Color32 c1, Color32 c2,Color32 c3,Color32 c4,
        Vector3 normal,
        List<HexCellVertexData> vertexBufferList)
    {
        AddTriangleWithNormal(a1, b2, b1, c2, c3, c4, normal,vertexBufferList);
        AddTriangleWithNormal(b1, a2, a1, c4, c1, c2,normal, vertexBufferList);
    }

    public static void AddQuad(Vector3 a2, Vector3 a1, Vector3 b2, Vector3 b1, Color32 c1, Color32 c2,
        List<HexCellVertexData> vertexBufferList)
    {
        AddTriangle(a1, b2, b1, c1, c2, c2, vertexBufferList);
        AddTriangle(b1, a2, a1, c2, c1, c1, vertexBufferList);
    }


    #endregion
    

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

    #region Add Single Vertex

    public static void AddSingleVertex(Vector3 pos, Vector3 normal, Color32 color,
        List<HexCellVertexData> vertexBufferList)
    {
        vertexBufferList.Add(new HexCellVertexData(pos, normal, color));
    }
    
    #endregion
    
}